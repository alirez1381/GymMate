using AutoMapper;
using GymMate.Data;
using GymMate.Models;
using GymMate.Models.DTO;
using GymMate.Repository.IRepository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Text;
using GymMate.Model;
using GymMate.Model.DTO;

namespace GymMate.Repository
{
    public class UserRepository : IUserRepository

        
    {
        private ApplicationDbContext _db;
        private string secretKey;
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _RoleManager;
        private IMapper _mapper;
        
        public UserRepository(ApplicationDbContext db, IConfiguration configuration , UserManager<ApplicationUser> userManager , IMapper mapper, RoleManager<IdentityRole>  roleManager)
        {
            _db = db;
            secretKey = configuration.GetValue<string>("ApiSetting:Secret");
            _userManager = userManager;
            _mapper = mapper;
            _RoleManager = roleManager;

        }
        public bool IsUniqueUser(string username)
        {
            var user= _db.ApplicationUsers.FirstOrDefault(x=>x.UserName == username);
            if (user == null)
            {
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequest)
        {
           var user= _db.ApplicationUsers.FirstOrDefault(u=>u.UserName.ToLower() == loginRequest.UserName.ToLower());
            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequest.Password);
            if(user == null || isValid == false )
            {
                return new LoginResponseDTO()
                {
                    Token ="",
                    User = null,
                };

            }

            //jwt
            var roles = await _userManager.GetRolesAsync(user);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)



            };
            var token = tokenHandler.CreateToken(tokenDescription);
            LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
            {
                Token = tokenHandler.WriteToken(token),
                User = _mapper.Map<UserDTO>(user) ,
                Role = roles.FirstOrDefault(),
               
            };
            return loginResponseDTO;
        }

        public async Task<UserDTO> Register(RegisterationRequestDTO registerationRequestDTO)
        {

            ApplicationUser user = new()
            {
                UserName = registerationRequestDTO.Username,
               
                Email = registerationRequestDTO.Username,
               
               

            };

           var result = await _userManager.CreateAsync(user,registerationRequestDTO.Password);
            if (result.Succeeded)
            {
                await SendConfirmationEmail(user);
                if (!_RoleManager.RoleExistsAsync("user").GetAwaiter().GetResult())
                {
                    await _RoleManager.CreateAsync(new IdentityRole("User"));
                }

                await _userManager.AddToRoleAsync(user, "User");
                var userToReturn = _db.ApplicationUsers.FirstOrDefault(u=>u.UserName == registerationRequestDTO.Username);
                return new UserDTO
                {
                    Id = userToReturn.Id,
                    UserName = userToReturn.UserName,
                    
                };
            }

            return new UserDTO();
        }

        public async Task<bool> SendConfirmationEmail(ApplicationUser user)
        {
            Random random = new Random();
            string verificationCode = random.Next(100000, 999999).ToString(); // کد 6 رقمی

            user.EmailVerificationCode = verificationCode;
            user.EmailConfirmExpireDataTime = DateTime.UtcNow.AddMinutes(4); // مثلا 2 دقیقه مهلت داره
            await _userManager.UpdateAsync(user);

            MailMessage mailmessage = new MailMessage("tangestani35@gmail.com", user.Email);
            mailmessage.Subject = "کد تایید حساب کاربری";
            mailmessage.IsBodyHtml = true;
            mailmessage.Body = $"<h3>کد تایید شما:</h3><h2>{verificationCode}</h2>";

            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new NetworkCredential("tangestani35@gmail.com", "nhbh jfzg dufg vtzd");
            smtp.EnableSsl = true;

            try
            {
                smtp.Send(mailmessage);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return false;

            Random random = new Random();
            string resetCode = random.Next(100000, 999999).ToString();

            user.PasswordResetCode = resetCode;
            user.PasswordResetExpireDate = DateTime.UtcNow.AddMinutes(5);
            await _userManager.UpdateAsync(user);

            MailMessage mailMessage = new MailMessage("tangestani35@gmail.com", user.Email)
            {
                Subject = "بازیابی رمز عبور",
                Body = $"<h3>کد بازیابی رمز عبور شما:</h3><h2>{resetCode}</h2>",
                IsBodyHtml = true
            };

            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("tangestani35@gmail.com", "nhbh jfzg dufg vtzd"),
                EnableSsl = true
            };

            try
            {
                smtp.Send(mailMessage);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> ResetPasswordAsync(string email, string code, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || user.PasswordResetCode != code || user.PasswordResetExpireDate < DateTime.UtcNow)
            {
                return false;
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (result.Succeeded)
            {
                // بعد از ریست موفق، کد رو پاک کنیم
                user.PasswordResetCode = null;
                user.PasswordResetExpireDate = null;
                await _userManager.UpdateAsync(user);
                return true;
                }

            return false;
        }



    }
}
