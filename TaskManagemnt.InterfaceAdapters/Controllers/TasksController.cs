using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TaskManagemnt.Entities;
using TaskManagemnt.UseCases.Services;
namespace TaskManagemnt.InterfaceAdapters.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly TaskService _taskService;
    private readonly IValidator<TaskItem> _validator;
    public TasksController(TaskService taskService, IValidator<TaskItem> validator)
    {

        _taskService = taskService;
        _validator = validator;
    }
    [HttpPost]
    public async Task<IActionResult> Create(TaskItem taskItem)
    {
        var validationResult = await _validator.ValidateAsync(taskItem);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);
        var taskId = await _taskService.AddTaskAsync(taskItem);

        return CreatedAtAction(nameof(GetById), new { taskId }, taskItem);
    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) 
    { 
        var taskItem = await _taskService.GetTaskByIdAsync(id);

        if (taskItem == null)
            return NotFound();

        return Ok(taskItem);
    }
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(TaskItem taskItem, int id)
    {
        var existTask = await _taskService.GetTaskByIdAsync(id);

        if (existTask == null)
            return NotFound();

        existTask.Title = taskItem.Title;
        existTask.Description = taskItem.Description;
        existTask.DueDate = taskItem.DueDate;
        existTask.CreateDate = taskItem.CreateDate;
        existTask.IsCompleted = taskItem.IsCompleted;

        await _taskService.UpdateTaskAsync(existTask);

        return Ok(existTask);
    }
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _taskService.DeleteTaskAsync(id);
        if (!deleted)
            return NotFound();

        return NoContent();

    }
}
