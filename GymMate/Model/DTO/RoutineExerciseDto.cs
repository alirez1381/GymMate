namespace GymMate.Model.DTO
{
    public class RoutineExerciseDto
    {
        public int ExerciseId { get; set; }
        public string Name { get; set; }
        public int Reps { get; set; }
        public float? Weight { get; set; }
        public int Set { get; set; }
    }
}
