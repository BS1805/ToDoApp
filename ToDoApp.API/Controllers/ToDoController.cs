using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Application.DTOs;
using ToDoApp.Application.Interfaces;
using System.Threading.Tasks;

namespace ToDoApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ToDoController : ControllerBase
    {
        private readonly IToDoService _toDoService;

        public ToDoController(IToDoService toDoService)
        {
            _toDoService = toDoService;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetTasksForUser(
         string userId,
         [FromQuery] int page = 1,
         [FromQuery] int pageSize = 10)
        {
            var tasks = await _toDoService.GetPagedToDoItemsAsync(userId, page, pageSize);
            return Ok(tasks);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id, [FromQuery] string userId)
        {
            try
            {
                var task = await _toDoService.GetTaskViewModelForUser(id, userId);
                return Ok(task);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] TaskViewModel model, [FromQuery] string userId)
        {
            var task = await _toDoService.CreateToDoItem(model, userId);
            return CreatedAtAction(nameof(GetTaskById), new { id = task.Id, userId }, task);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskViewModel model, [FromQuery] string userId)
        {
            try
            {
                var updatedTask = await _toDoService.UpdateToDoItem(model, userId);
                return Ok(updatedTask);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id, [FromQuery] string userId)
        {
            try
            {
                await _toDoService.DeleteToDoItemForUser(id, userId);
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpGet("statuses")]
        public async Task<IActionResult> GetStatuses()
        {
            var statuses = await _toDoService.GetStatusesAsync();
            return Ok(statuses);
        }
    }
}