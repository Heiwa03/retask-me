using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
﻿using System.IdentityModel.Tokens.Jwt;


namespace ReTaskMe.Controllers;
    public class BaseController : ControllerBase
    {
        protected Guid? UserGuid
        {
            get
            {
                var idString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                // Use TryParse for safer conversion and return the nullable Guid directly
                if (Guid.TryParse(idString, out var guid))
                {
                    return guid;
                }

                // Returns null if idString was null or if parsing failed
                return null;
            }
        }

        protected Guid? TestUserGuid
        {
            get
            {
                return Guid.Parse("BD2526E1-C0F9-48F4-B16C-537FD27795AF");
            }
        }
    }