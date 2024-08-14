using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanApp.Infrastructure.DTOs.Auth
{
    public class VerifyOTPDto
    {
        [EmailAddress]
        public string Email { get; set; }
        public int OTP { get; set; }
    }
}
