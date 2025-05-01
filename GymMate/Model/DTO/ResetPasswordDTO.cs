namespace GymMate.Model.DTO
{
    public class ResetPasswordDTO
    {
        public string Email { get; set; }
        public string Code { get; set; }
        public string NewPassword { get; set; }
        public string Token { get; set; }
    }
}
