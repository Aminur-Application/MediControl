using MediControl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediControl.BindingModels
{
    public class PublicUserInfo
    {
        public int UserClearanceNumber { get; set; }
        public string Username { get; set; }
        public IList<string> role { get; set; }
    }
}
