using CleanApp.Domain.CommonEntities;
using System.Collections.Generic;

namespace CleanApp.Domain.Entities
{
    public class Permission : BaseEntity
    {
        public string PermissionName { get; set; }
        public string Type { get; set; }
        public virtual ICollection<RolePermission> RolePermissions { get; set; }
    }
}
