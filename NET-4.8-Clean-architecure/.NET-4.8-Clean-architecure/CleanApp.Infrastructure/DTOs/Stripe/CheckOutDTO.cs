using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanApp.Infrastructure.DTOs.Stripe
{
    public class CheckOutDTO
    {
        public int UserId { get; set; }
        public List<string> PriceIds { get; set; }
    }
}
