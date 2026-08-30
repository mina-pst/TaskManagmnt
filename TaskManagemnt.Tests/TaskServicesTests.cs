using Moq;
using TaskManagemnt.Entities;
using TaskManagemnt.UseCases.Interfaces;
using TaskManagemnt.UseCases.Services;

namespace TaskManagement.Tests.Services;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly TaskService _service;

    public TaskServiceTests()
    {
        _repositoryMock = new Mock<ITaskRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _service = new TaskService(_repositoryMock.Object,_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task AddTaskAsync_ShouldAddTaskAndSaveChanges()
    {
        var taskItem = new TaskItem
        {
            Id = 1,
            Title = "Test Mina",
            Description = "I'ts for C#",
            CreateDate = DateTime.Now
        };

        _repositoryMock.Setup(x => x.AddAsync(taskItem)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _service.AddTaskAsync(taskItem);
        Assert.Equal(1, result);
        Assert.NotEqual(default,taskItem.CreateDate);
        _repositoryMock.Verify(x => x.AddAsync(taskItem),Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),Times.Once);
    }

    [Fact]
    public async Task GetTaskByIdAsync_ShouldReturnTask_WhenTaskExists()
    {
        var taskItem = new TaskItem
        {
            Id = 1,
            Title = "Test Mina"
        };

        _repositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(taskItem);

        var result = await _service.GetTaskByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result!.Id);
        Assert.Equal("Test Mina", result.Title);
    }

    [Fact]
    public async Task GetTaskByIdAsync_ShouldReturnNull_WhenTaskDoesNotExist()
    {
        _repositoryMock.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((TaskItem?)null);

        var result = await _service.GetTaskByIdAsync(999);
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateTaskAsync_ShouldUpdateTaskAndSaveChanges()
    {
        var taskItem = new TaskItem
        {
            Id = 1,
            Title = "Updated task"
        };

        _repositoryMock.Setup(x => x.UpdateAsync(taskItem)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _service.UpdateTaskAsync(taskItem);

        _repositoryMock.Verify(x => x.UpdateAsync(taskItem),Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),Times.Once);
    }

    [Fact]
    public async Task DeleteTaskAsync_ShouldDeleteTask_WhenTaskExists()
    {
        var taskItem = new TaskItem
        {
            Id = 1,
            Title = "Test Mina"
        };

        _repositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(taskItem);
        _repositoryMock.Setup(x => x.DeleteAsync(1)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _service.DeleteTaskAsync(1);
        Assert.True(result);

        _repositoryMock.Verify(x => x.GetByIdAsync(1),Times.Once);
        _repositoryMock.Verify(x => x.DeleteAsync(1),Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),Times.Once);
    }

    [Fact]
    public async Task DeleteTaskAsync_ShouldReturnFalse_WhenTaskDoesNotExist()
    {
        _repositoryMock.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((TaskItem?)null);
        var result = await _service.DeleteTaskAsync(999);

        Assert.False(result);

        _repositoryMock.Verify(x => x.DeleteAsync(1), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),Times.Never);
    }
}