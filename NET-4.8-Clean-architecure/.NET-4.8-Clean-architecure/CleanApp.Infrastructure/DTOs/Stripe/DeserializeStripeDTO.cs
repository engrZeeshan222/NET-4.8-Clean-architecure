using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanApp.Infrastructure.DTOs.Stripe
{
    public class DeserializeStripeDTO
    {
        public string id { get; set; }
        public string @object { get; set; }
        public string api_version { get; set; }
        public int created { get; set; }
        public Data data { get; set; }
        public bool livemode { get; set; }
        public string type { get; set; }
    }
    public class Object
    {
        public string id { get; set; }

        public bool captured { get; set; }
        public int created { get; set; }
        public string currency { get; set; }
        public string customer { get; set; }
        public CustomerDetails customer_details { get; set; }
        public string client_reference_id { get; set; }
        public string description { get; set; }
        public object destination { get; set; }
        public object dispute { get; set; }
        public bool disputed { get; set; }
        public object failure_balance_transaction { get; set; }
        public object failure_code { get; set; }
        public object failure_message { get; set; }
        public string invoice { get; set; }


    }
    public class Data
    {
        public Object @object { get; set; }
    }
    public class CustomerDetails
    {
        public string email { get; set; }
        // Other properties you want to deserialize
    }
}
