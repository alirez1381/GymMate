using System.ComponentModel.DataAnnotations;

namespace GymMate.Models.DTO
{
    public class LoginRequestDTO
    {
        [Required]
        [EmailAddress]
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
