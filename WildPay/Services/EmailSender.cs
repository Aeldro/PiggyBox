using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net;

namespace WildPay.Services
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Créer un objet MailMessage
            MailMessage mail = new MailMessage
            {
                From = new MailAddress(Environment.GetEnvironmentVariable("BREVO_ID")),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };
            mail.To.Add(email);

            // Configurer le client SMTP
            SmtpClient smtpClient = new SmtpClient("smtp-relay.brevo.com", 587)
            {
                Credentials = new NetworkCredential(Environment.GetEnvironmentVariable("BREVO_ID"), Environment.GetEnvironmentVariable("BREVO_PASSWORD")),
                EnableSsl = true
            };

            try
            {
                // Envoyer l'email
                await smtpClient.SendMailAsync(mail);
                Console.WriteLine("Email envoyé avec succès !");
            }
            catch (Exception ex)
            {
                // Gestion des erreurs
                Console.WriteLine($"Erreur lors de l'envoi de l'email : {ex.Message}");
                throw;
            }
        }
    }
}
