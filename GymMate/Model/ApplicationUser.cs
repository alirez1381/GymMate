using Microsoft.AspNetCore.Identity;

namespace GymMate.Model
{
    public class ApplicationUser : IdentityUser
    {
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? Weight { get; set; }
        public string? EmailVerificationCode { get; set; }
        public DateTime? EmailConfirmExpireDataTime { get; set; }
        public string? PasswordResetCode { get; set; }
        public DateTime? PasswordResetExpireDate { get; set; }
        public ICollection<Routine> Routines { get; set; }

    }
}
