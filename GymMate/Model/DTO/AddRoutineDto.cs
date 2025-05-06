namespace GymMate.Model.DTO
{
    public class AddRoutineDto
    {
        public string Name { get; set; }
       public List<RoutineExerciseDto> Exercises { get; set; }
    }
}
