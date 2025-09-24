using BusinessLogicLayerCore.Services.Interfaces;
using HelperLayer.Security;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailHelper _emailHelper;

        public EmailService(EmailHelper emailHelper)
        {
            _emailHelper = emailHelper;
        }

        public Task<bool> SendEmailAsync(List<string> recipients, string subject, string htmlContent)
        {
            return _emailHelper.SendEmailAsync(recipients, subject, htmlContent);
        }
    }
}
