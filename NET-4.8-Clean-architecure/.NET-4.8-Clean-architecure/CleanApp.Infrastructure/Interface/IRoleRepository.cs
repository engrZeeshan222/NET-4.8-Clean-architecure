using CleanApp.Infrastructure.DTOs.Roles;
using System.Threading.Tasks;

namespace CleanApp.Infrastructure.Interface
{
    public interface IRoleRepository
    {
        Task<ResponseDto> CreateRole(AddRoleDto addRoleDto);
        Task<ResponseDto> UpdateRole(EditRoleDto editRoleDto);
        Task<ResponseDto> GetAllRole(int pageNo, int pageSize, string searchString, bool includeDeleted);
        Task<ResponseDto> GetRoleById(int id);
        Task<ResponseDto> DeleteRole(int id);
        Task<ResponseDto> RestoreRole(int id);
    }
}
