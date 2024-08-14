using CleanApp.Infrastructure;
using CleanApp.Infrastructure.DTOs.Auth;
using CleanApp.Infrastructure.Interface;
using CleanApp.Infrastructure.Repositories;
using System.Threading.Tasks;

namespace CleanApp.Application.Services.Auths
{
    public class AuthService: IAuthService
    {

        private readonly IAuthRepository authRepository;
        public AuthService()
        {
            this.authRepository = new AuthRepository();
        }
        //public AuthService(
        //   IAuthRepository authRepo)
        //{
        //    authRepo = authRepository;
        //}

        public async Task<ResponseDto> Login(LoginDto loginDto)
        {
            return await authRepository.Login(loginDto);
        }

        public async Task<ResponseDto> Signup(SignupDto signupDto)
        {
            return await this.authRepository.Signup(signupDto);
        } 
        public async Task<ResponseDto> SendSMSOTP(LoginDto loginDto)
        {
            return await this.authRepository.SendSMSOTP(loginDto);
        } 
        public async Task<ResponseDto> SendWhatsAppOTP(LoginDto loginDto)
        {
            return await this.authRepository.SendWhatsAppOTP(loginDto);
        }
        public async Task<ResponseDto> VerifyOTP(VerifyOTPDto otpDto)
        {
            return await this.authRepository.VerifyOTP(otpDto);
        }
    }
}
