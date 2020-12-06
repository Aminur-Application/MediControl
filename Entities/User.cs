using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;


namespace MediControl.Entities
{
    public class User : IdentityUser<Guid>
    {

         public string FirstName { get; set; }
        public string LastName { get; set; }
        public int ClearanceNumber { get; set; }


        [JsonIgnore]
        public List<RefreshToken> RefreshTokens { get; set; }
    }
}
