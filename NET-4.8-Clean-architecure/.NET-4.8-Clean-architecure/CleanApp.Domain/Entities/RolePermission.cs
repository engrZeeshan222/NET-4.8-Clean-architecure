using CleanApp.Domain.CommonEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanApp.Domain.Entities
{
    public class RolePermission:BaseEntity
    {
        [ForeignKey("RoleId")]
        public int RoleId { get; set; }
        public Role Role { get; set; }
        [ForeignKey("PermissionId")]
        public int PermissionId { get; set; }
        public Permission Permission { get; set; }
    }
}
