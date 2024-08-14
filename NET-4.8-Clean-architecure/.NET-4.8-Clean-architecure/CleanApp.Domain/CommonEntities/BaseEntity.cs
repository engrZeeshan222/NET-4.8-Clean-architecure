using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanApp.Domain.CommonEntities
{
    public class BaseEntity
    {
        public BaseEntity()
        {

            IsActive = true;
            IsDeleted = false;
            CreatedDateTime = DateTime.Now;
        }
        public int ID { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
    }
}

