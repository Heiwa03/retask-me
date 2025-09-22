using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
﻿using System.IdentityModel.Tokens.Jwt;


namespace ReTaskMe.Controllers;
    public class BaseController : ControllerBase{
        protected Guid? UserGuid{
            get{
                var idString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (idString == null) return null;

                return Guid.Parse(idString);
            }
        }

        protected Guid? TaskGuid{
            get{
                var idString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (idString == null) return null;

                return Guid.Parse(idString);
            }
        }
    }