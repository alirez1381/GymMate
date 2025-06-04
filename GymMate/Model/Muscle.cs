namespace GymMate.Model
{
    public class Muscle
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[]? Image { get; set; }


        public List<Exercise> Exercises { get; set; }
    }
}
