using GymMate.Data;
using GymMate.Model;
using GymMate.Model.DTO;
using GymMate.Models.DTO;
using GymMate.Repository;
using GymMate.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymMate.Controllers
{
    [Route("[controller]/[action]")]
    [Produces("application/json")]
    [ApiController]
    public class RoutineController : ControllerBase
    {
        private readonly IRoutineRepository _routineRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;

        public RoutineController(IRoutineRepository routineRepository, UserManager<ApplicationUser> userManager,ApplicationDbContext db)
        {
            _routineRepository = routineRepository;
            _userManager = userManager;
            _db = db;
        }
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddRoutine([FromBody] AddRoutineDto model)
        {

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var routine = await _routineRepository.AddRoutineAsync(user.Id, model);
            return Ok(new { routine.Id, routine.Name });
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<IActionResult> GetExercises()
        {
            var exercises = await _routineRepository.GetExercisesAsync();
            return Ok(exercises);
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllMuscles()
        {
            var muscles = await _routineRepository.GetAllMusclesAsync();
            return Ok(muscles);
        }


        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<IActionResult> UpdateRoutineName([FromBody] UpdateRoutineNameDto dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var result = await _routineRepository.UpdateRoutineNameAsync(user.Id, dto);
            return result ? Ok("Routine name updated.") : NotFound();
        }

        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<IActionResult> AddExerciseToRoutine([FromBody] AddExerciseToRoutineDto dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var result = await _routineRepository.AddExerciseToRoutineAsync(user.Id, dto);
            return result ? Ok("Exercise added.") : BadRequest("Exercise already exists or routine not found.");
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<IActionResult> UpdateExerciseInRoutine([FromBody] UpdateExerciseInRoutineDto dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var result = await _routineRepository.UpdateExerciseInRoutineAsync(user.Id, dto);
            return result ? Ok("Exercise updated.") : NotFound();
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<IActionResult> RemoveExerciseFromRoutine([FromBody] RemoveExerciseFromRoutineDto dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var result = await _routineRepository.RemoveExerciseFromRoutineAsync(user.Id, dto);
            return result ? Ok("Exercise removed.") : NotFound();
        }



        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<IActionResult> DeleteRoutine(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var result = await _routineRepository.DeleteRoutineAsync(user.Id, id);
            if (!result)
                return NotFound("Routine not found or access denied.");

            return NoContent(); 
        }


        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<IActionResult> GetMyRoutines()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var routines = await _routineRepository.GetUserRoutinesAsync(user.Id);
            return Ok(routines);
        }

        [HttpGet]
        [Authorize]
        [HttpGet("GetRoutine")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<RoutineDetailsDto>> GetRoutine(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var routine = await _db.Routines
                .Include(r => r.RoutineExercises)
                    .ThenInclude(re => re.Exercise)
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == user.Id);

            if (routine == null)
                return NotFound("Routine not found");

            var dto = new RoutineDetailsDto
            {
                Id = routine.Id,
                Name = routine.Name,
                Exercises = routine.RoutineExercises.Select(re => new ExerciseInRoutineDto
                {
                    ExerciseId = re.Id,
                    Name = re.Exercise?.Name,
                    Reps = re.Reps,
                    Sets = re.set,
                    Weight = re.Weight
                    
                }).ToList()
            };

            return Ok(dto);
        }


        [HttpPost("AddExercise")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddExercise([FromForm] AddExerciseDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Name) || model.ExerImg == null)
                return BadRequest("Name and image are required.");

            var result = await _routineRepository.AddExerciseAsync(model);
            if (!result)
                return BadRequest("Muscle group not found.");

            return Ok("Exercise added successfully.");
        }

        [HttpPost("AddMuscle")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddMuscle([FromForm] AddmuscelsDTO model)
        {
            var result = await _routineRepository.AddMuscleAsync(model);
            if (!result)
                return BadRequest("Failed to add muscle. It may already exist or input was invalid.");

            return Ok("Muscle added successfully.");
        }


        
    }
}
