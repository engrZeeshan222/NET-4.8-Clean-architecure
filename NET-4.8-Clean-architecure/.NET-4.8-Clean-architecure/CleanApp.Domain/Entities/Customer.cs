using CleanApp.Domain.CommonEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanApp.Domain.Entities
{
    public class Customer:BaseEntity
    {
        public string Email { get; set; }
        public string stripeCustomerId { get; set; }
        [ForeignKey("UserId")]
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
