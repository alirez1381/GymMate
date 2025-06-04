namespace GymMate.Model
{
    public class Exercise
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public int MuscleId { get; set; }
        public Muscle Muscle { get; set; }
        public byte[]? ExerciseImg { get; set; }


        public List<RoutineExercise>? RoutineExercises { get; set; }
    }
}
