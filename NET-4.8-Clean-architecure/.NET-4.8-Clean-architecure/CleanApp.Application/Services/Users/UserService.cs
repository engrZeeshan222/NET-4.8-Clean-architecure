using CleanApp.Infrastructure;
using CleanApp.Infrastructure.DTOs.Users;
using CleanApp.Infrastructure.Interface;
using CleanApp.Infrastructure.Repositories;
using CleanApplication.Infrastructure.Interface;
using CleanApplication.Infrastructure.Repositories;
using System.Threading.Tasks;

namespace CleanApp.Application.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;

        public UserService()
        {
            this.userRepository = new UserRepository();
        }

        public async Task<ResponseDto> CreateUser(AddUserDto addUserDto)
        {
            return await this.userRepository.CreateUser(addUserDto);
        }

        public Task<ResponseDto> DeleteUser(int id)
        {
            return this.userRepository.DeleteUser(id);
        }

        public async Task<ResponseDto> GetAllUsers(int pageNo, int pageSize, string searchString, bool includeDeleted)
        {
            return await this.userRepository.GetAllUsers(pageNo, pageSize, searchString, includeDeleted);
        }

        public async Task<ResponseDto> GetUserById(int id)
        {
            return await this.userRepository.GetUserById(id);
        }

        public async Task<ResponseDto> RestoreUser(int id)
        {
            return await this.userRepository.RestoreUser(id);
        }

        public async Task<ResponseDto> UpdateUser(EditUserDto editUserDto)
        {
            return await this.userRepository.UpdateUser(editUserDto);
        }
    }
}
