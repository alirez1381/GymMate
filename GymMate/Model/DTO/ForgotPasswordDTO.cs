using System.ComponentModel.DataAnnotations;

namespace GymMate.Model.DTO
{
    public class ForgotPasswordDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
