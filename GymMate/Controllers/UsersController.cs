using GymMate.Model;
using GymMate.Model.DTO;
using GymMate.Models;
using GymMate.Models.DTO;
using GymMate.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Magic_Villa.Controllers
{
    [Route("api/UsersApi")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(IUserRepository userRepository, UserManager<ApplicationUser> userManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            var loginResponse = await _userRepository.Login(model);
            if (loginResponse.User == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                return BadRequest(new { message = "Invalid username or password." });
            }
            return Ok(loginResponse);
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Register([FromBody] RegisterationRequestDTO model)
        {
            bool isUnique = _userRepository.IsUniqueUser(model.Username);
            if (!isUnique)
            {
                return BadRequest(new { message = "Username already exists." });
            }

            var user = await _userRepository.Register(model);
            if (user == null)
            {
                return BadRequest(new { message = "Registration failed. Please try again." });
            }
            return Ok(user);
        }

        
        [HttpPost("verify-email")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequestDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return NotFound(new { message = "User not found." });

            if (user.EmailVerificationCode != model.Code)
                return BadRequest(new { message = "Invalid verification code." });

            if (user.EmailConfirmExpireDataTime < DateTime.UtcNow)
                return BadRequest(new { message = "Verification code has expired." });

            user.EmailConfirmed = true;
            user.EmailVerificationCode = null;
            user.EmailConfirmExpireDataTime = null;

            await _userManager.UpdateAsync(user);

            return Ok(new { message = "Email successfully verified." });
        }


        [HttpPost("forgot-password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO model)
        {
            var isSuccess = await _userRepository.ForgotPasswordAsync(model.Email);
            if (!isSuccess)
                return BadRequest(new { message = "Failed to send reset password email." });

            return Ok(new { message = "Reset password email sent successfully." });
        }

        [HttpPost("reset-password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO model)
        {
            var isSuccess = await _userRepository.ResetPasswordAsync(model.Email, model.Code, model.NewPassword);
            if (!isSuccess)
                return BadRequest(new { message = "Failed to reset password. The code is invalid or expired." });

            return Ok(new { message = "Password has been reset successfully." });
        }



        [HttpGet("GetProfile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProfile([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email is required");

            var user = await _userRepository.GetProfile(email);

            if (user == null)
                return NotFound("User not found");

            return Ok(user);
        }

        [HttpPut("UpdateProfile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProfile([FromForm] ProfileDTO model)
        {
            if (string.IsNullOrWhiteSpace(model.UserName))
                return BadRequest("Email/UserName is required.");

            var result = await _userRepository.UpdateProfile(model);

            if (!result)
                return NotFound("User not found.");

            return Ok("Profile updated successfully.");
        }
    }
}
