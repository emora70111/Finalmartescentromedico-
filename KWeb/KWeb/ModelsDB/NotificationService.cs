using System;
using System.Timers;

public class NotificacionService
{
    private static Timer _timer;


    public static void Iniciar()
    {

        _timer = new Timer(24 * 60 * 60 * 1000);
        _timer.Elapsed += EnviarNotificaciones;
        _timer.AutoReset = true;
        _timer.Enabled = true;
    }



    private static void EnviarNotificaciones(object sender, ElapsedEventArgs e)
    {
        var servicioCorreo = new EmailService();


        string destinatario = "correo-destinatario@example.com";
        string asunto = "Notificación Automática";
        string mensaje = "Este es un mensaje de prueba enviado automáticamente.";


        servicioCorreo.SendEmailAsync(destinatario, asunto, mensaje).Wait();
    }
}