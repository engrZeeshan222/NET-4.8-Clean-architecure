using CleanApp.Infrastructure;
using CleanApp.Infrastructure.DTOs.Auth;
using System.Threading.Tasks;

namespace CleanApp.Application.Services.Auths
{
    public  interface IAuthService
    {
        Task<ResponseDto> Signup(SignupDto signupDto);
        Task<ResponseDto> Login(LoginDto loginDto);
        Task<ResponseDto> SendSMSOTP(LoginDto loginDto);
        Task<ResponseDto> SendWhatsAppOTP(LoginDto loginDto);
        Task<ResponseDto> VerifyOTP(VerifyOTPDto otpDto);
    }
}
