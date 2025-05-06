namespace GymMate.Model
{
    public class Muscle
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[]? Image { get; set; }


        public ICollection<Exercise> Exercises { get; set; }
    }
}
