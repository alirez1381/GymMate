namespace GymMate.Model.DTO
{
    public class AddExerciseDto
    {
      
        public string Name { get; set; }
        public string MuscleGroupName { get; set; }
        public string Description { get; set; }
        public IFormFile ExerImg { get; set; }

    }
}
