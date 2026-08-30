

using TaskManagemnt.Entities;

namespace TaskManagemnt.UseCases.Interfaces
{
   public interface ITaskRepository
    {
        Task<TaskItem?> GetByIdAsync(int Id);
        Task AddAsync(TaskItem item);
        Task UpdateAsync(TaskItem item);
        Task DeleteAsync(TaskItem item);
    }
}
