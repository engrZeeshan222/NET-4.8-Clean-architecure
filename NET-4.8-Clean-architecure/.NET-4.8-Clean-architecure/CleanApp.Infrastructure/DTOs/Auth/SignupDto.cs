using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanApp.Infrastructure.DTOs.Auth
{
    public class SignupDto
    {
        public string FirstName { get; set; }
        public string SurName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        
    }
}
