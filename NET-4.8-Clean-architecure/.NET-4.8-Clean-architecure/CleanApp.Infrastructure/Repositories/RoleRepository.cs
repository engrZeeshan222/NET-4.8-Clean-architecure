using CleanApp.Domain.Entities;
using CleanApp.Infrastructure;
using CleanApp.Infrastructure.DTOs.Roles;
using CleanApp.Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CleanApplication.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDbContext db;

        public RoleRepository()
        {
            db = SingletonContext.Instance;
        }
        public async Task<ResponseDto> CreateRole(AddRoleDto addRoleDto)
        {
            var response = new ResponseDto();
            try
            {
                var isExist = await IsUserRoleExist(addRoleDto.RoleName);
                if (isExist != null)
                {
                    response.Status = false;
                    response.Message = "Role Already Exist";
                    return response;
                }
                var role = new Role()
                {
                    RoleName = addRoleDto.RoleName,

                };
                await this.db.Roles.AddAsync(role);
                await this.db.SaveChangesAsync();
                response.Status = true;
                response.Message = "Role Added Successfully";
            }
            catch (Exception ex)
            {
                response.Message = ex.InnerException?.Message;
            }
            return response;
        }

        public async Task<ResponseDto> UpdateRole(EditRoleDto editRoleDto)
        {
            ResponseDto response = new ResponseDto();
            try
            {
                var role = await GetRole(editRoleDto.ID);
                if (role != null)
                {
                    var isExist = await IsUserRoleExist(editRoleDto.RoleName);
                    if (isExist != null)
                    {
                        response.Status = false;
                        response.Message = "Role Already Exist";
                        return response;
                    }
                    role.RoleName = editRoleDto.RoleName;
                    role.UpdatedDateTime = DateTime.UtcNow;
                    await this.db.SaveChangesAsync();
                    response.Status = true;
                    response.Message = "Success";
                }
                else
                {
                    response.Status = false;
                    response.Message = "No Record Found";
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            }
            return response;
        }

        public async Task<ResponseDto> GetAllRole(int pageNo, int pageSize, string searchString, bool includeDeleted)
        {
            var response = new ResponseDto();
            try
            {
                var query = this.db.Roles.Where(x => x.IsDeleted == includeDeleted)
                    .Select(u => new
                    {
                        u.ID,
                        u.RoleName,
                        u.IsDeleted
                    });
                int totalCount = query.Count();
                if (!string.IsNullOrEmpty(searchString))
                {
                    query = query.Where(x => x.RoleName.Contains(searchString));
                }
                var role = await query
                    .OrderBy(o => o.ID)
                    .Skip((pageNo - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
                var responseData = new
                {
                    TotalCounts = totalCount,
                    Roles = role
                };
                response.Status = true;
                response.Message = "Success";
                response.Data = responseData;


            }
            catch (Exception ex)
            {
                response.Message = ex.InnerException == null ? ex.Message : ex.InnerException.Message;

            }
            return response;
        }

        public async Task<ResponseDto> GetRoleById(int id)
        {
            ResponseDto response = new ResponseDto();
            try
            {
                var role = await GetRole(id);
                if (role != null)
                {
                    var ResponseData = new
                    {
                        role.RoleName,
                        role.ID
                    };
                    response.Data = ResponseData;
                    response.Status = true;
                    response.Message = "Success";
                }
                else
                {
                    response.Status = false;
                    response.Message = "No Record Found";
                }

            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return response;
        }

        public async Task<ResponseDto> DeleteRole(int id)
        {
            ResponseDto response = new ResponseDto();
            try
            {
                var role = await GetRole(id);
                if (role != null)
                {
                    if (!role.IsDeleted)
                    {
                        DeleteOrRestoreRole(role, true);
                        await this.db.SaveChangesAsync();
                        response.Status = true;
                        response.Message = "Success";
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = "Role Already Deleted";
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = "No Record Found";
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            }
            return response;
        }
        public async Task<ResponseDto> RestoreRole(int id)
        {
            ResponseDto response = new ResponseDto();
            try
            {
                var user = await GetRole(id);
                if (user != null)
                {
                    if (user.IsDeleted)
                    {
                        DeleteOrRestoreRole(user, false);
                        await this.db.SaveChangesAsync();
                        response.Status = true;
                        response.Message = "Success";
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = $"Role is Already Restore";
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = "No Record Found";
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            }
            return response;
        }

        private void DeleteOrRestoreRole(Role role, bool flag)
        {
            role.IsDeleted = flag;
            role.UpdatedDateTime = DateTime.UtcNow;
        }

        public async Task<Role> GetRole(int Id)
        {
            return await this.db.Roles.FirstOrDefaultAsync(u => u.ID == Id);
        }

        private async Task<Role> IsUserRoleExist(string roleName)
        {
            return await this.db.Roles.Where(x => x.RoleName.Equals(roleName) && x.IsActive == true).FirstOrDefaultAsync();
        }
    }
}
