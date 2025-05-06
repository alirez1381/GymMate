namespace GymMate.Model
{
    public class RoutineExercise
    {
        public int Id { get; set; }

        public int RoutineId { get; set; }
        public Routine Routine { get; set; }

        public int ExerciseId { get; set; }
        public Exercise Exercise { get; set; }

        public int Reps { get; set; }
        public float? Weight { get; set; }
        public int set { get; set; }
    }
}
