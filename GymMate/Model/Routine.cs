namespace GymMate.Model
{
    public class Routine
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }

        // روابط
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public List<RoutineExercise> RoutineExercises { get; set; }
    }
}
