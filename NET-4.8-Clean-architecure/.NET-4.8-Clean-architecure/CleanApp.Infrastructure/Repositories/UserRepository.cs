using CleanApp.Domain.Entities;
using CleanApp.Infrastructure;
using CleanApp.Infrastructure.DTOs.Users;
using CleanApplication.Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CleanApplication.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext db;

        public UserRepository()
        {
            db = SingletonContext.Instance;
        }

        public async Task<ResponseDto> CreateUser(AddUserDto addUserDto)
        {
            var response = new ResponseDto();
            try
            {
                var isExist = await IsUserEmailExist(addUserDto.Email);
                if (isExist != null)
                {
                    response.Message = "User Email Already Exist";
                    return response;
                }
                var role = await this.db.Roles.FirstOrDefaultAsync(r => r.ID == addUserDto.RoleId);
                if (role == null)
                {
                    response.Message = "No Role Found";
                    return response;
                }
                var user = new User()
                {
                    Email = addUserDto.Email,
                    FirstName = addUserDto.FirstName,
                    SurName = addUserDto.SurName,
                    PhoneNumber = addUserDto.PhoneNumber,
                    Password = addUserDto.Password,
                    RoleId = addUserDto.RoleId,
                };
                await this.db.Users.AddAsync(user);
                await this.db.SaveChangesAsync();
                response.Status = true;
                response.Message = "User Added SuccessFully";
            }
            catch (Exception ex)
            {

                response.Message = ex.InnerException?.Message;
            }
            return response;

        }

        public async Task<ResponseDto> DeleteUser(int id)
        {
            ResponseDto response = new ResponseDto();
            try
            {
                var user = await GetUser(id);
                if (user != null)
                {
                    if (!user.IsDeleted)
                    {
                        DeleteOrRestoreUser(user, true);
                        await this.db.SaveChangesAsync();
                        response.Status = true;
                        response.Message = "Success";
                    }
                    else
                    {
                        response.Message = $"User is Already Retired";
                    }
                }
                else
                {
                    response.Message = "No Record Found";
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            }
            return response;
        }

        public async Task<ResponseDto> GetAllUsers(int pageNo, int pageSize, string searchString, bool includeDeleted)
        {
            var response = new ResponseDto();
            try
            {
                //CreateProjectUsingCmd crud = new CreateProjectUsingCmd();
                //crud.CreateProjectCmd();
                var query = this.db.Users.Where(u => u.IsDeleted == includeDeleted)
                       .Select(u => new
                       {
                           u.ID,
                           u.SurName,
                           u.Email,
                           u.Password,
                           u.FirstName,
                           u.PhoneNumber,
                           u.IsDeleted
                       });
                int totalCount = query.Count();
                if (!string.IsNullOrEmpty(searchString))
                {
                    query = query.Where(u => u.FirstName.Contains(searchString));
                }
                var user = await query
                    .OrderBy(u => u.ID)
                    .Skip((pageNo - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
                var responseData = new
                {
                    TotalCounts = totalCount,
                    Users = user
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

        public async Task<ResponseDto> GetUserById(int id)
        {
            ResponseDto response = new ResponseDto();
            try
            {
                var user = await GetUser(id);

                if (user != null)
                {

                    var responseData = new
                    {
                        user.ID,
                        user.FirstName,
                        user.Email,
                        user.SurName,
                        user.Password,
                        user.PhoneNumber,
                        user.RoleId,
                    };

                    response.Data = responseData;
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

        public async Task<ResponseDto> RestoreUser(int id)
        {
            ResponseDto response = new ResponseDto();
            try
            {
                var user = await GetUser(id);
                if (user != null)
                {
                    if (user.IsDeleted)
                    {
                        DeleteOrRestoreUser(user, false);
                        await this.db.SaveChangesAsync();
                        response.Status = true;
                        response.Message = "Success";
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = $"User is Already Restore";
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

        public async Task<ResponseDto> UpdateUser(EditUserDto editUserDto)
        {
            ResponseDto response = new ResponseDto();
            try
            {

                var user = await GetUser(editUserDto.ID);
                if (user != null)
                {
                    var isExist = await IsUserEmailExist(editUserDto.Email);
                    if (isExist != null && isExist.Email != user.Email)
                    {
                        response.Message = "User Email Already Exist";
                        return response;
                    }
                    var role = await this.db.Roles.FirstOrDefaultAsync(r => r.ID == editUserDto.RoleId);
                    if (role == null)
                    {
                        response.Message = "No Role Found";
                        return response;
                    }
                    user.Email = editUserDto.Email;
                    user.FirstName = editUserDto.FirstName;
                    user.SurName = editUserDto.SurName;
                    user.PhoneNumber = editUserDto.PhoneNumber;
                    user.Password = editUserDto.Password;
                    user.RoleId = editUserDto.RoleId;
                    user.UpdatedDateTime = DateTime.UtcNow;
                    await this.db.SaveChangesAsync();
                    response.Status = true;
                    response.Message = "Success";
                }
                else
                {
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
        public async Task<User> GetUser(int Id)
        {
            return await this.db.Users
                .Include(u => u.Role)
               .FirstOrDefaultAsync(u => u.ID == Id);
        }
        private void DeleteOrRestoreUser(User user, bool flag)
        {
            user.IsDeleted = flag;
            user.UpdatedDateTime = DateTime.UtcNow;
        }
        private async Task<User> IsUserEmailExist(string email)
        {
            return await this.db.Users.Where(x => x.Email.Equals(email) && x.IsActive == true).FirstOrDefaultAsync();

        }
    }
}
