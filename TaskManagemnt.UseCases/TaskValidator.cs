using FluentValidation;
using TaskManagemnt.Entities;

namespace TaskManagemnt.UseCases
{
    public class TaskValidator:AbstractValidator<TaskItem>
    {
        public TaskValidator() 
        {
            RuleFor(t => t.Title).NotEmpty().MaximumLength(200);
            RuleFor(d=> d.Description).MaximumLength(1000);
        }
    }
}
