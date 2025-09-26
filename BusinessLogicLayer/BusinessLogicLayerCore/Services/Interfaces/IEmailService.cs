using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogicLayerCore.Services.Interfaces
{
    public interface IEmailService
    {
        // Send a generic email
        Task<bool> SendEmailAsync(List<string> recipients, string subject, string htmlContent);

        // Send a verification email to a single recipient
        Task<bool> SendVerificationEmailAsync(string recipient, string verificationLink);
    }
}
