using GymMate.Model.DTO;
using GymMate.Models;
using GymMate.Models.DTO;

namespace GymMate.Repository.IRepository
{
    public interface IUserRepository
    {
        bool IsUniqueUser(string username);
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequest);
        Task<UserDTO> Register(RegisterationRequestDTO registerationRequest);
        Task<bool> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string code, string newPassword);
        Task<ProfileDTO?> GetProfile(string email);
        Task<bool> UpdateProfile(ProfileDTO model);





    }
}
