using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Configuration;

namespace KWeb.Services
{
    public class EmailService
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPassword;
        private readonly bool _enableSsl;

        public EmailService()
        {          
            _smtpHost = ConfigurationManager.AppSettings["SmtpHost"];
            _smtpPort = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);
            _smtpUser = ConfigurationManager.AppSettings["SmtpUser"];
            _smtpPassword = ConfigurationManager.AppSettings["SmtpPassword"];
            _enableSsl = bool.Parse(ConfigurationManager.AppSettings["EnableSsl"]);
        }

        public bool EnviarCorreo(string correoDestino, string asunto, string bodyHtml)
        {
            try
            {
                using (var client = new SmtpClient(_smtpHost, _smtpPort))
                {
                    client.Credentials = new NetworkCredential(_smtpUser, _smtpPassword);
                    client.EnableSsl = _enableSsl;

                    using (var message = new MailMessage())
                    {
                        message.From = new MailAddress(_smtpUser);
                        message.To.Add(correoDestino);
                        message.Subject = asunto;
                        message.Body = bodyHtml;
                        message.IsBodyHtml = true;

                        client.Send(message);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error enviando correo: {ex.Message}");
                return false;
            }
        }
    }
}