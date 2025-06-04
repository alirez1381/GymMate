using GymMate.Model;
using GymMate.Model.DTO;

namespace GymMate.Repository.IRepository
{
    public interface IRoutineRepository
    {
        Task<Routine> AddRoutineAsync(string userId, AddRoutineDto model);
        Task<IEnumerable<ExerciseDto>> GetExercisesAsync();
        Task<IEnumerable<Muscle>> GetAllMusclesAsync();
        Task<bool> DeleteRoutineAsync(string userId, int routineId);
        Task<IEnumerable<RoutineDetailsDto>> GetUserRoutinesAsync(string userId);
        Task<bool> UpdateRoutineNameAsync(string userId, UpdateRoutineNameDto dto);
        Task<bool> AddExerciseToRoutineAsync(string userId, AddExerciseToRoutineDto dto);
        Task<bool> UpdateExerciseInRoutineAsync(string userId, UpdateExerciseInRoutineDto dto);
        Task<bool> RemoveExerciseFromRoutineAsync(string userId, RemoveExerciseFromRoutineDto dto);
         Task<bool> AddExerciseAsync(AddExerciseDto model);
        Task<bool> AddMuscleAsync(AddmuscelsDTO model);
        


    }
}
