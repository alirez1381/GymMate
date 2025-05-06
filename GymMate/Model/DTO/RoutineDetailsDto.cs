namespace GymMate.Model.DTO
{
    public class RoutineDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ExerciseInRoutineDto> Exercises { get; set; }
    }
}
