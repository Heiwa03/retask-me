using BusinessLogicLayerCore.Services.Interfaces;
using HelperLayer.Security;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogicLayerCore.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailHelper? _emailHelper;

        // Allows DI to construct EmailService even if EmailHelper is not registered (fallback/no-op)
        public EmailService() { }

        public EmailService(EmailHelper emailHelper)
        {
            _emailHelper = emailHelper;
        }

        public Task<bool> SendEmailAsync(List<string> recipients, string subject, string htmlContent)
        {
            if (_emailHelper == null)
            {
                // No email configuration available; succeed without sending
                return Task.FromResult(true);
            }

            return _emailHelper.SendEmailAsync(recipients, subject, htmlContent);
        }
    }
}
