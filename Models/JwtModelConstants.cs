using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediControl.Models
{
    public class JwtModelConstants
    {
        public const string Issuer = "Medicontrol";
        public const string Audience = "ApiUser";
        public const string Key = "D5QKKQb05uoGENIqG";


        public const string AuthSchemes = "Identity.Application" + "," + JwtBearerDefaults.AuthenticationScheme;
    }
}
