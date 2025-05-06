namespace GymMate.Model.DTO
{
    public class ExerciseToRoutineDto
    {
        public int RoutineId { get; set; }
        public int ExerciseId { get; set; }
        public int Sets { get; set; }
        public int Reps { get; set; }
        public float? Weight { get; set; }
    }
}
