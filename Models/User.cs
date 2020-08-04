using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediControl.Models
{
    public class User : IdentityUser<Guid>
    {
        public int ClearanceNumber { get; set; }
    }
}
