using GymMate.Model;
using GymMate.Model.DTO;
using GymMate.Models.DTO;
using GymMate.Repository;
using GymMate.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GymMate.Controllers
{
    [Route("[controller]/[action]")]
    [Produces("application/json")]
    [ApiController]
    public class RoutineController : ControllerBase
    {
        private readonly IRoutineRepository _routineRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public RoutineController(IRoutineRepository routineRepository, UserManager<ApplicationUser> userManager)
        {
            _routineRepository = routineRepository;
            _userManager = userManager;
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddRoutine([FromBody] AddRoutineDto model)
        {

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var routine = await _routineRepository.AddRoutineAsync(user.Id, model);
            return Ok(new { routine.Id, routine.Name });
        }

        [HttpGet]
        public async Task<IActionResult> GetExercises([FromQuery] string? muscleName, [FromQuery] string? search)
        {
            var exercises = await _routineRepository.GetExercisesAsync(muscleName, search);
            return Ok(exercises);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllMuscles()
        {
            var muscles = await _routineRepository.GetAllMusclesAsync();
            return Ok(muscles);
        }


        [HttpPost()]
        [Authorize]
        public async Task<IActionResult> UpdateRoutineName([FromBody] UpdateRoutineNameDto dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var result = await _routineRepository.UpdateRoutineNameAsync(user.Id, dto);
            return result ? Ok("Routine name updated.") : NotFound();
        }

        [HttpPost()]
        [Authorize]
        public async Task<IActionResult> AddExerciseToRoutine([FromBody] AddExerciseToRoutineDto dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var result = await _routineRepository.AddExerciseToRoutineAsync(user.Id, dto);
            return result ? Ok("Exercise added.") : BadRequest("Exercise already exists or routine not found.");
        }

        [HttpPost()]
        [Authorize]
        public async Task<IActionResult> UpdateExerciseInRoutine([FromBody] UpdateExerciseInRoutineDto dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var result = await _routineRepository.UpdateExerciseInRoutineAsync(user.Id, dto);
            return result ? Ok("Exercise updated.") : NotFound();
        }

        [HttpPost()]
        [Authorize]
        public async Task<IActionResult> RemoveExerciseFromRoutine([FromBody] RemoveExerciseFromRoutineDto dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var result = await _routineRepository.RemoveExerciseFromRoutineAsync(user.Id, dto);
            return result ? Ok("Exercise removed.") : NotFound();
        }



        [HttpPost]
        [Authorize]
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


        [HttpGet()]
        [Authorize]
        public async Task<IActionResult> GetMyRoutines()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var routines = await _routineRepository.GetUserRoutinesAsync(user.Id);
            return Ok(routines);
        }

    }
}
