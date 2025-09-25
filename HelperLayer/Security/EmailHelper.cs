using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure;
using Azure.Communication.Email;

namespace HelperLayer.Security
{
    public class EmailHelper
    {
        private readonly EmailClient _emailClient;
        private readonly string _senderAddress;

        public EmailHelper(EmailClient emailClient, string senderAddress)
        {
            _emailClient = emailClient;
            _senderAddress = senderAddress;
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

                var emailMessage = new EmailMessage(_senderAddress, recipients, emailContent);

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
    }
}
