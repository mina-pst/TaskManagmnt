using Microsoft.EntityFrameworkCore;
using TaskManagemnt.Entities;
using TaskManagemnt.UseCases.Interfaces;
namespace TaskManagemnt.Infrastructure.Repositories;
public class TaskRepository : ITaskRepository
{
    private readonly TaskDbContext _context;
    public TaskRepository(TaskDbContext context)
    {
        _context = context;
    }
    public async Task<TaskItem?> GetByIdAsync(int id)
    {
        return await _context.Tasks.FirstOrDefaultAsync(x => x.Id == id);
    }
    public async Task AddAsync(TaskItem taskItem)
    {
        await _context.Tasks.AddAsync(taskItem);
    }
    public Task UpdateAsync(TaskItem taskItem)
    {
        _context.Tasks.Update(taskItem);

        return Task.CompletedTask;
    }
    public Task DeleteAsync(TaskItem taskItem)
    {
        _context.Tasks.Remove(taskItem);

        return Task.CompletedTask;
    }
}