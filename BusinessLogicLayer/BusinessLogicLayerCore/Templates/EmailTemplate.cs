using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayerCore.Templates
{
    public static class EmailTemplates
    {
        public static string WelcomeTemplate(string bodyContent)
        {
            return $@"
            <html>
                <body>
                    <h2>Welcome!</h2>
                    {bodyContent}
                    <p>Thank you for joining us.</p>
                </body>
            </html>";
        }

        public static string VerificationTemplate(string frontendUrl, string jwtToken)
        {
            string link = $"{frontendUrl}/verify-email?token={jwtToken}";
            string bodyContent = $@"
            <p>Please verify your email:</p>
            <p><a href='{link}'>Verify Email</a></p>
            <p>If you did not register, ignore this email.</p>";

            return WelcomeTemplate(bodyContent);
        }
    }

}


