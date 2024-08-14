using CleanApp.Infrastructure;
using CleanApp.Infrastructure.DTOs.Users;
using System.Threading.Tasks;

namespace CleanApp.Application.Services.Users
{
    public interface IUserService
    {
        Task<ResponseDto> CreateUser(AddUserDto addUserDto);
        Task<ResponseDto> GetAllUsers(int pageNo, int pageSize, string searchString, bool includeDeleted);
        Task<ResponseDto> GetUserById(int id);
        Task<ResponseDto> UpdateUser(EditUserDto editUserDto);
        Task<ResponseDto> DeleteUser(int id);
        Task<ResponseDto> RestoreUser(int id);
    }
}
