namespace GymMate.Model.DTO
{
    public class ExerciseDto
    {
        public int exerciseId { get; set; } 
        public string Name { get; set; }
        public string MuscleGroupName { get; set; }
        public string Description { get; set; }
        public string ExerImgBase64 { get; set; } // یا ExerImgUrl


    }
}

