using GymMate.Data;
using GymMate.Model;
using GymMate.Model.DTO;
using GymMate.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace GymMate.Repository
{
    public class RoutineRepository: IRoutineRepository
    {
        private readonly ApplicationDbContext _context;

        public RoutineRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Routine> AddRoutineAsync(string userId, AddRoutineDto model)
        {
            var routine = new Routine
            {
                Name = model.Name,
                CreatedAt = DateTime.UtcNow,
                UserId = userId,
                RoutineExercises = model.Exercises.Select(e => new RoutineExercise
                {
                    ExerciseId = e.ExerciseId,
                    Reps = e.Reps,
                    Weight = e.Weight,
                    set = e.Set,
                }).ToList()
            };

            _context.Routines.Add(routine);
            await _context.SaveChangesAsync();
            return routine;
        }
        public async Task<IEnumerable<ExerciseDto>> GetExercisesAsync()
        {
            var exercises = await _context.Exercises.Include(e=>e.Muscle).ToListAsync(); 

            var result = exercises.Select(e => new ExerciseDto
            {
                ExersiseId=e.Id,
                Name = e.Name,
                Description = e.Description,
                MuscleGroupName = e.Muscle?.Name,
                ExerImgBase64 = e.ExerciseImg != null ? Convert.ToBase64String(e.ExerciseImg) : null

            });


            return result;
        }

        public async Task<bool> UpdateRoutineNameAsync(string userId, UpdateRoutineNameDto dto)
        {
            var routine = await _context.Routines.FirstOrDefaultAsync(r => r.Id == dto.RoutineId && r.UserId == userId);
            if (routine == null) return false;

            routine.Name = dto.NewName;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddExerciseToRoutineAsync(string userId, AddExerciseToRoutineDto dto)
        {
            var routine = await _context.Routines
                .Include(r => r.RoutineExercises)
                .FirstOrDefaultAsync(r => r.Id == dto.RoutineId && r.UserId == userId);

            if (routine == null || routine.RoutineExercises.Any(e => e.ExerciseId == dto.ExerciseId))
                return false;

            routine.RoutineExercises.Add(new RoutineExercise
            {
                ExerciseId = dto.ExerciseId,
                set = dto.Sets,
                Reps = dto.Reps,
                Weight = dto.Weight
            });

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateExerciseInRoutineAsync(string userId, UpdateExerciseInRoutineDto dto)
        {
            var routineExercise = await _context.RoutineExercises
                .Include(re => re.Routine)
                .FirstOrDefaultAsync(re => re.RoutineId == dto.RoutineId &&
                                           re.ExerciseId == dto.ExerciseId &&
                                           re.Routine.UserId == userId);

            if (routineExercise == null) return false;

            routineExercise.set = dto.Sets;
            routineExercise.Reps = dto.Reps;
            routineExercise.Weight = dto.Weight;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveExerciseFromRoutineAsync(string userId, RemoveExerciseFromRoutineDto dto)
        {
            var routineExercise = await _context.RoutineExercises
                .Include(re => re.Routine)
                .FirstOrDefaultAsync(re => re.RoutineId == dto.RoutineId &&
                                           re.ExerciseId == dto.ExerciseId &&
                                           re.Routine.UserId == userId);

            if (routineExercise == null) return false;

            _context.RoutineExercises.Remove(routineExercise);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Muscle>> GetAllMusclesAsync()
        {
            return await _context.Muscles.ToListAsync();
        }

        public async Task<bool> DeleteRoutineAsync(string userId, int routineId)
        {
            var routine = await _context.Routines
                .Include(r => r.RoutineExercises)
                .FirstOrDefaultAsync(r => r.Id == routineId && r.UserId == userId);

            if (routine == null)
                return false;

            _context.RoutineExercises.RemoveRange(routine.RoutineExercises);
            _context.Routines.Remove(routine);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<IEnumerable<RoutineDetailsDto>> GetUserRoutinesAsync(string userId)
        {
            return await _context.Routines
                .Where(r => r.UserId == userId)
                .Include(r => r.RoutineExercises)
                .ThenInclude(re => re.Exercise)
                .Select(r => new RoutineDetailsDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Exercises = r.RoutineExercises.Select(re => new ExerciseInRoutineDto
                    {
                        ExerciseId = re.ExerciseId,
                        Name = re.Exercise.Name,
                        Sets = re.set,
                        Reps = re.Reps,
                        Weight = re.Weight
                    }).ToList()
                })
                .ToListAsync();
        }


      

        public async Task<bool> AddExerciseAsync(AddExerciseDto model)
        {
            var muscle = await _context.Muscles.FirstOrDefaultAsync(m => m.Name.ToLower() == model.MuscleGroupName.ToLower());
            if (muscle == null)
                return false;

            
            var exercise = new Exercise
            {
                Name = model.Name,
                MuscleId = muscle.Id,
                Description = model.Description, 
                
            };
            if (model.ExerImg != null)
            {
                byte[] b = new byte[model.ExerImg.Length];
                model.ExerImg.OpenReadStream().Read(b);
                exercise.ExerciseImg = b;


            }


            await _context.Exercises.AddAsync(exercise);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> AddMuscleAsync(AddmuscelsDTO model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                return false;

            
            bool exists = await _context.Muscles
                .AnyAsync(m => m.Name.ToLower() == model.Name.ToLower());

            if (exists)
                return false;

            var muscle = new Muscle
            {
                Name = model.Name
            };

            if (model.MuscelImg != null && model.MuscelImg.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await model.MuscelImg.CopyToAsync(memoryStream);
                muscle.Image = memoryStream.ToArray();
            }

            await _context.Muscles.AddAsync(muscle);
            await _context.SaveChangesAsync();
            return true;
        }



    }
}
