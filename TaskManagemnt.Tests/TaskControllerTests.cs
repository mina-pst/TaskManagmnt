using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManagemnt.Entities;
using TaskManagemnt.InterfaceAdapters.Controllers;
using TaskManagemnt.UseCases.Interfaces;
using TaskManagemnt.UseCases.Services;

namespace TaskManagement.Tests.Controllers;

public class TasksControllerTests
{
    private readonly Mock<ITaskRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IValidator<TaskItem>> _validatorMock;
    private readonly TaskService _taskService;
    private readonly TasksController _controller;

    public TasksControllerTests()
    {
        _repositoryMock = new Mock<ITaskRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _validatorMock = new Mock<IValidator<TaskItem>>();
        _taskService = new TaskService(_repositoryMock.Object,_unitOfWorkMock.Object);
        _controller = new TasksController(_taskService,_validatorMock.Object);
    }
    [Fact]
    public async Task Create_ShouldReturnCreated_WhenTaskIsValid()
    {
        var taskItem = new TaskItem
        {
            Title = "Test task"
        };

        _validatorMock.Setup(x => x.ValidateAsync(taskItem,It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
        _repositoryMock.Setup(x => x.AddAsync(taskItem)).Callback<TaskItem>(x => x.Id = 1).Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _controller.Create(taskItem);
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);

        Assert.Equal(nameof(TasksController.GetById), createdResult.ActionName);
        Assert.Equal(taskItem, createdResult.Value);
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenTaskIsInvalid()
    {
        var taskItem = new TaskItem
        {
            Title = ""
        };

        var validationResult = new ValidationResult(
            new[]
            {
                new ValidationFailure("Title", "Title is required")
            });

        _validatorMock.Setup(x => x.ValidateAsync(taskItem,It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);

        var result = await _controller.Create(taskItem);
        Assert.IsType<BadRequestObjectResult>(result);

        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<TaskItem>()),Times.Never);
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_WhenTaskExists()
    {
        var taskItem = new TaskItem
        {
            Id = 1,
            Title = "Test task"
        };

        _repositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(taskItem);

        var result = await _controller.GetById(1);
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(taskItem, okResult.Value);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenTaskDoesNotExist()
    {
        _repositoryMock.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((TaskItem?)null);

        var result = await _controller.GetById(999);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Update_ShouldReturnOk_WhenTaskExists()
    {
        var existingTask = new TaskItem
        {
            Id = 1,
            Title = "Old task"
        };

        var updatedTask = new TaskItem
        {
            Id = 1,
            Title = "Updated task"
        };

        _repositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(existingTask);

        _repositoryMock.Setup(x => x.UpdateAsync(It.IsAny<TaskItem>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _controller.Update(updatedTask,1);
        var okResult = Assert.IsType<OkObjectResult>(result);
        var resultTask = Assert.IsType<TaskItem>(okResult.Value);

        Assert.Equal(1, resultTask.Id);
        Assert.Equal("Updated task", resultTask.Title);
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenTaskDoesNotExist()
    {
        _repositoryMock.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((TaskItem?)null);

        var taskItem = new TaskItem
        {
            Title = "Updated task"
        };

        var result = await _controller.Update(taskItem, 999);
        Assert.IsType<NotFoundResult>(result);

        _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<TaskItem>()),Times.Never);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenTaskIsDeleted()
    {
        var taskItem = new TaskItem
        {
            Id = 1,
            Title = "Test task"
        };

        _repositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(taskItem);
        _repositoryMock.Setup(x => x.DeleteAsync(1)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _controller.Delete(1);
        Assert.IsType<NoContentResult>(result);

        _repositoryMock.Verify(x => x.DeleteAsync(1),Times.Once);
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenTaskDoesNotExist()
    {
        _repositoryMock.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((TaskItem?)null);

        var result = await _controller.Delete(999);
        Assert.IsType<NotFoundResult>(result);

        _repositoryMock.Verify(x => x.DeleteAsync(It.IsAny<int>()),Times.Never);
    }
}