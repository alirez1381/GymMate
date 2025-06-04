namespace GymMate.Model.DTO
{
    public class ProfileDTO
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public IFormFile Image { get; set; }

    }
}
