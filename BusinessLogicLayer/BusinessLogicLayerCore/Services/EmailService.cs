using BusinessLogicLayerCore.Services.Interfaces;
using BusinessLogicLayerCore.Templates;
using HelperLayer.Security;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogicLayerCore.Services
{
    public class EmailService : IEmailService
    {
        //private readonly EmailHelper _emailHelper;
        private readonly string _defaultSender;

        // Fallback constructor for testing / missing EmailHelper
        public EmailService() { }

        public EmailService(string defaultSender)
        {
            //_emailHelper = emailHelper;
            _defaultSender = defaultSender;
        }

        public Task<bool> SendEmailAsync(List<string> recipients, string subject, string htmlContent)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sends a raw email with subject & HTML content.
        /// </summary>
        /*public Task<bool> SendEmailAsync(List<string> recipients, string subject, string htmlContent)
        {
            if (_emailHelper == null)
            {
                Console.WriteLine($"[Fallback Email] To: {string.Join(",", recipients)}, Subject: {subject}");
                return Task.FromResult(true);
            }

            return _emailHelper.SendEmailAsync(recipients, subject, htmlContent);
        }
        */
        /// <summary>
        /// Builds and sends a verification email for new users.
        /// </summary>
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
