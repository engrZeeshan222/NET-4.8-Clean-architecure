using CleanApp.Infrastructure.DTOs.Auth;
using System.Threading.Tasks;

namespace CleanApp.Infrastructure.Interface
{
    public interface IAuthRepository
    {
         Task<ResponseDto> Signup(SignupDto signupDto);
         Task<ResponseDto> Login(LoginDto loginDto);
        Task<ResponseDto> SendSMSOTP(LoginDto loginDto);
        Task<ResponseDto> SendWhatsAppOTP(LoginDto loginDto);
        Task<ResponseDto> VerifyOTP(VerifyOTPDto otpDto);
    }
}
