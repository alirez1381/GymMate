using System.ComponentModel.DataAnnotations;

namespace GymMate.Models.DTO
{
    public class RegisterationRequestDTO
    {
        [Required]
        [EmailAddress]
        public string Username { get; set; }
        [Required]
       
        public string Password { get; set; }
        [Required]
        public string Role { get; set; }
        
    }
}
