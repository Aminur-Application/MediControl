using MediControl.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediControl.Controllers.Api
{   

    [Authorize(AuthenticationSchemes = JwtModelConstants.AuthSchemes)]
    public class CustomerController : Controller
    {
        [HttpGet]
        public List<string> GetCustomers()
        {
            return new List<string>() { "Customer1", "Customer2" };
        }
    }
}
