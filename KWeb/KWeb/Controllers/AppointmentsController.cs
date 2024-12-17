using KWeb.Models.Request;
using KWeb.ModelsDB;
using KWeb.Services;
using KWeb.Utils;
using KWeb.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KWeb.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly CONSULTORIO_VIDA_SALUDEntities _context;
        private readonly EmailSystem emailSystem;

        public AppointmentsController()
        {
            emailSystem = new EmailSystem();
            _context = new CONSULTORIO_VIDA_SALUDEntities();
        }

        public ActionResult OfDoctor()
        {
            var userJson = HttpContext.Session["user"] as string;
            var user = JsonConvert.DeserializeObject<Users>(userJson);
            int DoctorId = user.Doctors.First().DoctorID;
            string Canceled = Enum.GetName(typeof(KWeb.Utils.AppointmentsStatusEnum), KWeb.Utils.AppointmentsStatusEnum.Canceled);

            var appointments = _context.Appointments
                .Where(a => DbFunctions.TruncateTime(a.Date) == DateTime.Today && a.DoctorID == DoctorId && a.Status != Canceled)
                .ToList();

            var tomorrow = DateTime.Today.AddDays(1);
            var appointmentsScheduled = _context.Appointments
                .Where(a => DbFunctions.TruncateTime(a.Date) >= tomorrow && a.DoctorID == DoctorId)
                .ToList();

            var viewModel = new AppointmentsOfDoctorViewModel
            {
                AppointmentsScheduled = appointmentsScheduled,
                AppointmentsToday = appointments
            };

            return View(viewModel);
        }
        public async Task<ActionResult> OfPatient()
        {
            var userJson = HttpContext.Session["user"] as string;
            var user = JsonConvert.DeserializeObject<Users>(userJson);
            int PatientId = user.Patients.First().PatientID;

            var appointments = await _context.Appointments.Where(a => a.PatientID == PatientId).ToListAsync();
            var doctors = await _context.Doctors.ToListAsync();

            var viewModel = new AppointmentsOfPatientViewModel
            {     
                Doctors = doctors,
                AppointmentRequest = new AppointmentRequest()
                {
                    Date = DateTime.Now.AddDays(1)
                }
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> Register(AppointmentsOfPatientViewModel vm)
        {
            AppointmentRequest model = vm.AppointmentRequest;

            var userJson = HttpContext.Session["user"] as string;
            var user = JsonConvert.DeserializeObject<Users>(userJson);

            model.PatientID = user.Patients.First().PatientID; 

            if (ModelState.IsValid)
            {
                var newAppointment = new Appointments
                {
                    PatientID = model.PatientID,
                    DoctorID = model.DoctorID,
                    Date = model.Date,
                    BlockID = model.BlockID,
                    Status = Enum.GetName(typeof(AppointmentsStatusEnum), AppointmentsStatusEnum.Scheduled),
                    Notes = model.Notes
                };

                _context.Appointments.Add(newAppointment);
                await _context.SaveChangesAsync();

                var appointmentRecently = _context.Appointments
                    .Include(a => a.Patients)
                     .Include(p => p.Patients.Users) // Incluir la relación con Users dentro de Patient
                    .Include(a => a.Doctors)
                    .Include(a => a.Blocks)
                    .FirstOrDefault(a => a.AppointmentID == newAppointment.AppointmentID);

                TempData["SuccessMessage"] = "La cita se agendo correctamente.";

                emailSystem.ScheduledPatientAppointment(appointmentRecently.Patients.Users.Email, appointmentRecently);
                emailSystem.ScheduledDoctorAppointment(appointmentRecently.Doctors.Users.Email, appointmentRecently);

                return RedirectToAction("OfPatient");
            }

            TempData["ErrorMessage"] = "No se pudo agendar la cita.";

            return RedirectToAction("OfPatient");
            
        }

        [HttpPost]
        public ActionResult AttendAppointment(int appointmentId, string notes, int blockId, int doctorId)
        {
            var appointment = _context.Appointments.FirstOrDefault(a => a.AppointmentID == appointmentId);

            var medicalHistory = new MedicalHistory
            {
                AppointmentId = appointmentId,
                ConsultationDate = DateTime.Now.Date,
                Notes = notes
            };
            _context.MedicalHistory.Add(medicalHistory);

            appointment.Status = Enum.GetName(typeof(KWeb.Utils.AppointmentsStatusEnum), KWeb.Utils.AppointmentsStatusEnum.Completed);

            _context.SaveChanges();

            return RedirectToAction("OfDoctor");
        }

        [HttpPost]
        public ActionResult CancelAppointment(int appointmentId)
        {
            var appointment = _context.Appointments
                   .Include(a => a.Patients)
                    .Include(p => p.Patients.Users)
                   .Include(a => a.Doctors)
                   .Include(a => a.Blocks)
                   .FirstOrDefault(a => a.AppointmentID == appointmentId);

            appointment.Status = Enum.GetName(typeof(KWeb.Utils.AppointmentsStatusEnum), KWeb.Utils.AppointmentsStatusEnum.Canceled);

            _context.SaveChanges();

            emailSystem.CanceledPatientAppointment(appointment.Patients.Users.Email, appointment);

            return RedirectToAction("OfDoctor");
        }

    }
}