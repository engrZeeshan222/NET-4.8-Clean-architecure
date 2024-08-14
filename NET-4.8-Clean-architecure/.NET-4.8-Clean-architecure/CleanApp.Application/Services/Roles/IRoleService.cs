using CleanApp.Infrastructure;
using CleanApp.Infrastructure.DTOs.Roles;
using System.Threading.Tasks;

namespace CleanApp.Application.Services.Roles
{
    public interface IRoleService
    {
        Task<ResponseDto> CreateRole(AddRoleDto addRoleDto);
        Task<ResponseDto> UpdateRole(EditRoleDto editRoleDto);
        Task<ResponseDto> GetAllRole(int pageNo, int pageSize, string searchString, bool includeDeleted);
        Task<ResponseDto> GetRoleById(int id);
        Task<ResponseDto> DeleteRole(int id);
        Task<ResponseDto> RestoreRole(int id);
    }
}
