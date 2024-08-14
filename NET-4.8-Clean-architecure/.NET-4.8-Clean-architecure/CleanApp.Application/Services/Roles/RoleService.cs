using CleanApp.Application.Services.Roles;
using CleanApp.Infrastructure;
using CleanApp.Infrastructure.DTOs.Roles;
using CleanApp.Infrastructure.Interface;
using CleanApplication.Infrastructure.Repositories;
using System.Threading.Tasks;

namespace CleanApplication.Application.Services.Roles
{
    public   class RoleService:IRoleService
    {
        private readonly IRoleRepository roleRepository;
        public RoleService()
        {
            this.roleRepository = new RoleRepository();
        }

        public async Task<ResponseDto> CreateRole(AddRoleDto addRoleDto)
        {
            return await this.roleRepository.CreateRole(addRoleDto);
        }

        public async Task<ResponseDto> DeleteRole(int id)
        {
            return await this.roleRepository.DeleteRole(id);
        }

        public async Task<ResponseDto> GetAllRole(int pageNo, int pageSize, string searchString, bool includeDeleted)
        {
            return await this.roleRepository.GetAllRole(pageNo, pageSize, searchString, includeDeleted);
        }

        public async Task<ResponseDto> GetRoleById(int id)
        {
            return await this.roleRepository.GetRoleById(id);
        }

        public async Task<ResponseDto> RestoreRole(int id)
        {
            return await this.roleRepository.RestoreRole(id);
        }

        public async Task<ResponseDto> UpdateRole(EditRoleDto editRoleDto)
        {
            return await this.roleRepository.UpdateRole(editRoleDto);
        }


    }
}
