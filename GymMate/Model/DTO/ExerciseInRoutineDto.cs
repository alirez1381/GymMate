namespace GymMate.Model.DTO
{
    public class ExerciseInRoutineDto
    {
        public int ExerciseId { get; set; }
        public int RoutineId { get; set; }
        public int Sets { get; set; }
        public int Reps { get; set; }
        public float? Weight { get; set; }
        public string Name { get; set; }
    }
}
