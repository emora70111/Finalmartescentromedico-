using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

public class EmailService
{
    
    private readonly string smtpHost = "smtp.ufide.ac.cr"; 
    private readonly int smtpPort = 587; 
    private readonly string senderEmail = "smontero50021@ufide.ac.cr"; 
    private readonly string senderPassword = "Proyecto1234"; 

     public async Task SendEmailAsync(string recipient, string subject, string body)
    {
        
        using (var client = new SmtpClient(smtpHost, smtpPort))
        {
            client.Credentials = new NetworkCredential(senderEmail, senderPassword);
            client.EnableSsl = true; 

            
            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true 
            };

            mailMessage.To.Add(recipient); 

            
            await client.SendMailAsync(mailMessage);
        }
    }
}
