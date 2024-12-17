using KWeb.ModelsDB;
using KWeb.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KWeb.Utils
{
    public class EmailSystem
    {
        private readonly EmailService emailService;

        public EmailSystem()
        {
            emailService = new EmailService();
        }

        public bool InactivatedUserAppointment(string email, Users user)
        {
            bool enviado = emailService.EnviarCorreo(
                   correoDestino: email,
                   asunto: "Cuenta Desactivada",
                   bodyHtml: $@"
                       <h1 style='color: #2c3e50;'>Notificación de Cuenta Desactivada</h1>
                       <p>Estimado/a <strong>{user.FirstName} {user.LastName}</strong>,</p>
                       <p>Le informamos que su cuenta ha sido <strong>desactivada</strong> por un administrador del sistema.</p>
                       <p>En consecuencia, usted <strong>no podrá acceder</strong> al sistema hasta nuevo aviso.</p>
                       <p>Si tiene alguna duda o requiere más información, por favor, comuníquese con el área de soporte.</p>
                       <p>Atentamente,</p>
                       <p style='color: #2980b9;'><strong>Equipo de Soporte</strong></p>"
            );

            return enviado;
        }

        // Notificación de cita agendada al paciente
        public bool ScheduledPatientAppointment(string email, Appointments appointment)
        {
            bool enviado = emailService.EnviarCorreo(
                   correoDestino: email,
                   asunto: "Confirmación de Cita Médica",
                   bodyHtml: $@"
                       <h1 style='color: #2c3e50;'>Cita Médica Programada</h1>
                       <p>Estimado/a Paciente,</p>
                       <p>Le confirmamos que su cita ha sido <strong>programada</strong> con éxito.</p>
                       <p><strong>Detalles de la Cita:</strong></p>
                       <ul>
                           <li><strong>Fecha:</strong> {appointment.Date:dd/MM/yyyy}</li>
                           <li><strong>Hora:</strong> {appointment.Blocks.BlockStartTime}</li>
                       </ul>
                       <p>Por favor, asegúrese de llegar al menos 10 minutos antes de la hora programada.</p>
                       <p>Para cualquier consulta adicional, no dude en contactarnos.</p>
                       <p>Atentamente,</p>
                       <p style='color: #2980b9;'><strong>Centro Médico</strong></p>"
            );

            return enviado;
        }

        // Notificación de cita agendada al doctor
        public bool ScheduledDoctorAppointment(string email, Appointments appointment)
        {
            bool enviado = emailService.EnviarCorreo(
                   correoDestino: email,
                   asunto: "Nueva Cita Programada en su Agenda",
                   bodyHtml: $@"
                       <h1 style='color: #2c3e50;'>Notificación de Nueva Cita</h1>
                       <p>Estimado/a Doctor/a,</p>
                       <p>Se ha agendado una nueva cita en su calendario.</p>
                       <p><strong>Detalles de la Cita:</strong></p>
                       <ul>
                           <li><strong>Fecha:</strong> {appointment.Date:dd/MM/yyyy}</li>
                           <li><strong>Hora:</strong> {appointment.Blocks.BlockStartTime}</li>
                           <li><strong>Paciente:</strong> {appointment.Patients.Users.FirstName}</li>
                       </ul>
                       <p>Le solicitamos revisar su agenda y estar preparado/a para atender la consulta.</p>
                       <p>Atentamente,</p>
                       <p style='color: #2980b9;'><strong>Centro Médico</strong></p>"
            );

            return enviado;
        }

        // Notificación de cancelación al paciente
        public bool CanceledPatientAppointment(string email, Appointments appointment)
        {
            bool enviado = emailService.EnviarCorreo(
                   correoDestino:   email,
                   asunto: "Cita Médica Cancelada",
                   bodyHtml: $@"
                       <h1 style='color: #e74c3c;'>Cita Cancelada</h1>
                       <p>Estimado/a Paciente,</p>
                       <p>Lamentamos informarle que su cita programada para:</p>
                       <ul>
                           <li><strong>Fecha:</strong> {appointment.Date:dd/MM/yyyy}</li>
                           <li><strong>Hora:</strong> {appointment.Blocks.BlockStartTime}</li>
                       </ul>
                       <p>ha sido <strong>cancelada</strong> por el especialista.</p>
                       <p>Le invitamos a <strong>agendar una nueva cita</strong> a través de nuestro sistema o comunicándose con nosotros.</p>
                       <p>Pedimos disculpas por cualquier inconveniente.</p>
                       <p>Atentamente,</p>
                       <p style='color: #2980b9;'><strong>Centro Médico</strong></p>"
            );

            return enviado;
        }

        public bool RecoveryPassword(string email, string code)
        {
            bool enviado = emailService.EnviarCorreo(
                   correoDestino: email,
                   asunto: "Recuperacion de contraseña",
                   bodyHtml: $@"
                       <h1 style='color: #2c3e50;'>Codigo de recuperacion</h1>
                       <p>Estimado/a Paciente,</p>
                       <p>Este es su codigo unico para la recuperacion de su contraseña:</p>
                       <ul>
                           <li><strong>Codigo unico:</strong> {code}</li>                           
                       </ul>
                       "
            );

            return enviado;
        }
    }
}