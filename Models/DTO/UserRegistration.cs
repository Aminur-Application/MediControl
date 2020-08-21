using System;

namespace FloSelect.Web.Models.DTO {
    public class UserRegistration {
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordReentry { get; set; }
    }
}