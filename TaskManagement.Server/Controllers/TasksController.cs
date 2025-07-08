using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TaskManagement.Server.Controllers
{
    /// <summary>
    /// Controller for task management operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class TasksController(ITaskService taskService) : ControllerBase
    {
        /// <summary>
        /// Gets all tasks for the authenticated user
        /// </summary>
        /// <returns>Collection of user's tasks</returns>
        /// <response code="200">Tasks retrieved successfully</response>
        /// <response code="401">User not authenticated</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TaskResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetTasks()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var tasks = await taskService.GetUserTasksAsync(userId.Value);
            return Ok(tasks);
        }

        /// <summary>
        /// Gets a specific task by ID
        /// </summary>
        /// <param name="id">Task ID</param>
        /// <returns>Task details if found and user is authorized</returns>
        /// <response code="200">Task retrieved successfully</response>
        /// <response code="401">User not authenticated</response>
        /// <response code="404">Task not found or user not authorized</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTask(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var task = await taskService.GetTaskByIdAsync(id, userId.Value);
            return task == null ? NotFound() : Ok(task);
        }

        /// <summary>
        /// Creates a new task
        /// </summary>
        /// <param name="createDto">Task creation data</param>
        /// <returns>Created task</returns>
        /// <response code="201">Task created successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="401">User not authenticated</response>
        [HttpPost]
        [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateTask([FromBody] TaskCreateDto createDto)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            try
            {
                var task = await taskService.CreateTaskAsync(createDto, userId.Value);
                return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing task
        /// </summary>
        /// <param name="id">Task ID</param>
        /// <param name="updateDto">Task update data</param>
        /// <returns>Updated task</returns>
        /// <response code="200">Task updated successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="401">User not authenticated</response>
        /// <response code="404">Task not found or user not authorized</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskUpdateDto updateDto)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var task = await taskService.UpdateTaskAsync(id, updateDto, userId.Value);
            return task == null ? NotFound() : Ok(task);
        }

        /// <summary>
        /// Partially updates an existing task
        /// </summary>
        /// <param name="id">Task ID</param>
        /// <param name="patchDto">Task patch data</param>
        /// <returns>Updated task</returns>
        /// <response code="200">Task updated successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="401">User not authenticated</response>
        /// <response code="404">Task not found or user not authorized</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PatchTask(int id, [FromBody] TaskPatchDto patchDto)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var task = await taskService.PatchTaskAsync(id, patchDto, userId.Value);
            return task == null ? NotFound() : Ok(task);
        }

        /// <summary>
        /// Deletes a task
        /// </summary>
        /// <param name="id">Task ID</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">Task deleted successfully</response>
        /// <response code="401">User not authenticated</response>
        /// <response code="404">Task not found or user not authorized</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var deleted = await taskService.DeleteTaskAsync(id, userId.Value);
            return !deleted ? NotFound() : NoContent();
        }

        /// <summary>
        /// Gets the current user's ID from claims
        /// </summary>
        /// <returns>User ID if found, null otherwise</returns>
        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }
}
