using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusinessLogicLayerCore.DTOs
{
    public class EmailDTO
    {
        public List<string> Recipients { get; set; } = new();
        public string Subject { get; set; } = "Verify Your Email";
        public string BodyContent { get; set; } = "";
        public string TemplateName { get; set; } = "Welcome";
        public string JwtToken { get; set; } = "";
        public string FrontendUrl { get; set; } = "";
    }
}
