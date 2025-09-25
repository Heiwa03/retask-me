using BusinessLogicLayerCore.Services.Interfaces;
using HelperLayer.Security;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogicLayerCore.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailHelper? _emailHelper;

        // Fallback constructor for testing / missing EmailHelper
        public EmailService() { }

        public EmailService(EmailHelper emailHelper)
        {
            _emailHelper = emailHelper;
        }

        public Task<bool> SendEmailAsync(List<string> recipients, string subject, string htmlContent)
        {
            if (_emailHelper == null)
            {
                // Fallback: just log and succeed
                Console.WriteLine($"[Fallback Email] To: {string.Join(",", recipients)}, Subject: {subject}");
                return Task.FromResult(true);
            }

            return _emailHelper.SendEmailAsync(recipients, subject, htmlContent);
        }
    }

}
