using BusinessLogicLayerCore.Services.Interfaces;
using System;
using System.Collections.Generic;

using System.Threading.Tasks;

namespace BusinessLogicLayerCore.Services
{
    public class NoOpEmailService : IEmailService
    {
        public Task<bool> SendEmailAsync(List<string> recipients, string subject, string htmlContent)
        {
            Console.WriteLine($"[NoOp Email] To: {string.Join(", ", recipients)}, Subject: {subject}");
            return Task.FromResult(true);
        }

        public Task<bool> SendVerificationEmailAsync(string recipient, string verificationLink)
        {
            Console.WriteLine($"[NoOp Verification Email] To: {recipient}, Link: {verificationLink}");
            return Task.FromResult(true);
        }
    }
}
