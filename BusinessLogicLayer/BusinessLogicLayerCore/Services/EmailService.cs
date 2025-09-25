using BusinessLogicLayerCore.Services.Interfaces;
using BusinessLogicLayerCore.Templates;
using Azure;
using Azure.Communication.Email;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogicLayerCore.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailClient _emailClient;
        private readonly string _defaultSender;

        public EmailService(string connectionString, string defaultSender)
        {
            _emailClient = new EmailClient(connectionString);
            _defaultSender = defaultSender;
        }

        public async Task<bool> SendEmailAsync(List<string> recipientsEmails, string subject, string htmlContent)
        {
            if (recipientsEmails == null || recipientsEmails.Count == 0)
                throw new ArgumentException("At least one recipient is required.");

            try
            {
                var recipients = new EmailRecipients(
                    to: recipientsEmails.ConvertAll(email => new EmailAddress(email))
                );

                var emailContent = new EmailContent(subject)
                {
                    Html = htmlContent,
                    PlainText = "This is a fallback text version."
                };

                var emailMessage = new EmailMessage(_defaultSender, recipients, emailContent);

                await _emailClient.SendAsync(WaitUntil.Completed, emailMessage);

                Console.WriteLine($"Email sent to {string.Join(", ", recipientsEmails)}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex}");
                return false;
            }
        }

        public Task<bool> SendVerificationEmailAsync(string recipient, string verificationLink)
        {
            string bodyContent = "<p>Hi,</p>" +
                                 "<p>Please click the link below to verify your email:</p>" +
                                 $"<p><a href='{verificationLink}'>Verify Email</a></p>" +
                                 "<p>If you did not register, ignore this email.</p>";

            string htmlContent = EmailTemplates.WelcomeTemplate(bodyContent);

            return SendEmailAsync(
                new List<string> { recipient },
                "Verify Your Email",
                htmlContent
            );
        }
    }
}
