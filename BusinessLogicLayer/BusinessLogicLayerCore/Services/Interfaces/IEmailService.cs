using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayerCore.Services.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(List<string> recipients, string subject, string htmlContent);
    }
}
