using CleanApp.Domain.CommonEntities;
using System.Collections.Generic;

namespace CleanApp.Domain.Entities
{
    public class Role : BaseEntity
    {
        public string RoleName { get; set; }
        public virtual ICollection<RolePermission> RolePermissions { get; set; }
    }
}
