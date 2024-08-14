using CleanApp.Domain.Entities;
using CleanApp.Infrastructure.DTOs.Auth;
using CleanApp.Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace CleanApp.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext db;
        private string accountSid = ConfigurationManager.AppSettings["TwilioAccountSid"];
        private string authToken = ConfigurationManager.AppSettings["TwilioAuthToken"];
        private string twilioWhatsAppNumber = ConfigurationManager.AppSettings["TwilioWhatsAppNumber"];
        private string twilioPhoneNumber = ConfigurationManager.AppSettings["TwilioPhoneNumber"];

        public AuthRepository()
        {
            db = SingletonContext.Instance;
        }



        public async Task<ResponseDto> Login(LoginDto loginDto)
        {
            ResponseDto response = new ResponseDto();
            try
            {
                var IsExist = await IsUserEmailExist(loginDto.Email);
                if (IsExist != null)
                {
                    var IsPasswordExist = await IsEmailPasswordExist(loginDto);
                    if (IsPasswordExist != null)
                    {
                        //var token = GenerateJwtToken(loginDto.Email);
                        var logintoken = new
                        {
                            UserId = IsPasswordExist.ID
                        };
                        response.Status = true;
                        response.Message = "Success";
                        response.Data = logintoken;
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = "User Email Or Password is Incorrect";
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = "User Email Or Password is Incorrect";
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            }
            return response;
        }

        public async Task<ResponseDto> Signup(SignupDto signupDto)
        {
            ResponseDto response = new ResponseDto();
            try
            {
                var isExist = await IsUserEmailExist(signupDto.Email);
                if (isExist != null)
                {
                    response.Status = false;
                    response.Message = "User Email Already Exist";
                    return response;
                }
                var user = new User
                {
                    FirstName = signupDto.FirstName,
                    SurName = signupDto.SurName,
                    Email = signupDto.Email,
                    CreatedDateTime = DateTime.Now,
                    Password = signupDto.Password,
                    PhoneNumber = signupDto.PhoneNumber
                };
                await this.db.Users.AddAsync(user);
                await this.db.SaveChangesAsync();
                //var token = GenerateJwtToken(signupDto.Email);
                var logintoken = new
                {
                    UserId = user.ID
                };
                response.Status = true;
                response.Message = "User Signup Successfully";
                response.Data = logintoken;
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            }
            return response;
        }
        public async Task<ResponseDto> SendSMSOTP(LoginDto loginDto)
        {
            ResponseDto response = new ResponseDto();
            try
            {
                var IsExist = await IsUserEmailExist(loginDto.Email);
                if (IsExist != null)
                {
                    var IsPasswordExist = await IsEmailPasswordExist(loginDto);
                    if (IsPasswordExist != null)
                    {
                        int otp = GenerateOTP();
                        TwilioClient.Init(accountSid, authToken);
                        var recieverNumber = IsPasswordExist.PhoneNumber;
                        var messageOptions = new CreateMessageOptions(
                          new PhoneNumber(recieverNumber));
                        messageOptions.From = new PhoneNumber(twilioPhoneNumber);
                        messageOptions.Body = $"your otp code is:{otp}";
                        IsPasswordExist.OTP = otp;
                        IsPasswordExist.OTPExpireTime = DateTime.Now.AddMinutes(2);
                        await db.SaveChangesAsync();
                        var message = MessageResource.Create(messageOptions);
                        response.Status = true;
                        response.Message = "OTP SuccessFully Send";
                        response.Data = new { IsPasswordExist.FirstName, IsPasswordExist.Email };
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = "User Email Or Password is Incorrect";
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = "User Email Or Password is Incorrect";
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            }
            return response;
        }
        public async Task<ResponseDto> SendWhatsAppOTP(LoginDto loginDto)
        {
            ResponseDto response = new ResponseDto();
            try
            {
                var IsExist = await IsUserEmailExist(loginDto.Email);
                if (IsExist != null)
                {
                    var IsPasswordExist = await IsEmailPasswordExist(loginDto);
                    if (IsPasswordExist != null)
                    {
                        int otp = GenerateOTP();
                        TwilioClient.Init(accountSid, authToken);

                        var messageOptions = new CreateMessageOptions(
                          new PhoneNumber($"whatsapp:{IsExist.PhoneNumber}"));
                        messageOptions.From = new PhoneNumber(twilioWhatsAppNumber);
                        messageOptions.Body = $"your otp code is:{otp}";


                        MessageResource.Create(messageOptions);
                        IsPasswordExist.OTP = otp;
                        IsPasswordExist.OTPExpireTime = DateTime.Now.AddMinutes(2);
                        await db.SaveChangesAsync();
                        response.Status = true;
                        response.Message = "OTP SuccessFully Send";
                        response.Data = new { IsPasswordExist.FirstName, IsPasswordExist.Email };
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = "User Email Or Password is Incorrect";
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = "User Email Or Password is Incorrect";
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            }
            return response;
        }
        public async Task<ResponseDto> VerifyOTP(VerifyOTPDto otpDto)
        {
            ResponseDto response = new ResponseDto();
            try
            {
                var IsExist = await db.Users.FirstOrDefaultAsync(u => u.Email == otpDto.Email);
                if (IsExist != null)
                {
                    bool verified = false;
                    if (IsExist.OTP == otpDto.OTP && IsExist.OTPExpireTime > DateTime.Now)
                    {
                        verified = true;
                    }
                    if (verified)
                    {
                        //var token = GenerateJwtToken(otpDto.Email);
                        var logintoken = new
                        {
                            UserId = IsExist.ID
                        };
                        response.Status = true;
                        response.Message = "Success";
                        response.Data = logintoken;
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = "User Email Or Password is Incorrect";
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = "User Email Or Password is Incorrect";
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            }
            return response;
        }

        //private string GenerateJwtToken(string Email)
        //{
        //    var authClaims = new List<Claim>
        //    {
        //        new Claim (ClaimTypes.Name, Email),
        //        new Claim (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        //    };
        //    var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("VGVzdFN5c3RlbUlzUHJvZ3JhbQ=="));
        //    var token = new JwtSecurityToken(
        //     issuer: "your_issuer",
        //     audience: "your_audience",
        //     expires: DateTime.Now.AddDays(1),
        //     claims: authClaims,
        //     signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
        // );
        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}

        private async Task<User> IsUserEmailExist(string email)
        {
            return await this.db.Users.Where(x => x.Email.Equals(email) && x.IsActive == true).FirstOrDefaultAsync();
        }

        private async Task<User> IsEmailPasswordExist(LoginDto loginDto)
        {
            var user = await db.Users.Where(x => x.Email == loginDto.Email).FirstOrDefaultAsync();
            if (user != null && string.Equals(user.Password, loginDto.Password))
            {
                return user;
            }
            return null;
        }
        private static int GenerateOTP()
        {
            // Create a byte array to store the random bytes
            byte[] randomBytes = new byte[4]; // 4 bytes = 32 bits

            // Generate random bytes
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomBytes);
            }

            // Convert the bytes to a 6-digit OTP
            int otpNumber = Math.Abs(BitConverter.ToInt32(randomBytes, 0)) % 1000000;
            return otpNumber;
        }

    }
}
