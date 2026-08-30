using Microsoft.EntityFrameworkCore;
using TaskManagemnt.Infrastructure;
using TaskManagemnt.Infrastructure.Repositories;
using TaskManagemnt.UseCases.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TaskDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();