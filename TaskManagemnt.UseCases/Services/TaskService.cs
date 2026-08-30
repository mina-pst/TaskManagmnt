using System;
using System.Collections.Generic;
using System.Text;
using TaskManagemnt.Entities;
using TaskManagemnt.UseCases.Interfaces;

namespace TaskManagemnt.UseCases.Services
{
    public class TaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IUnitOfWork _unitOfWork;
        public TaskService(ITaskRepository taskRepository, IUnitOfWork unitOfWork)
        {
            _taskRepository = taskRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<int> AddTaskAsync(TaskItem item)
        {
            item.CreateDate = DateTime.Now;
            await _taskRepository.AddAsync(item);
            await _unitOfWork.SaveChangesAsync();

            return item.Id;
        }
        public async Task<TaskItem?> GetTaskByIdAsync(int id)
        {
            return await _taskRepository.GetByIdAsync(id);

        }
        public async Task UpdateTaskAsync(TaskItem item)
        {
            await _taskRepository.UpdateAsync(item);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task<bool> DeleteTaskAsync(int id)
        {
            var taskItem = await _taskRepository.GetByIdAsync(id);
            if (taskItem == null)
                return false;

            await _taskRepository.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
