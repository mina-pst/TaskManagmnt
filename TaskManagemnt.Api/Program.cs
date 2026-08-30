using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TaskManagemnt.Entities;
using TaskManagemnt.Infrastructure;
using TaskManagemnt.Infrastructure.Repositories;
using TaskManagemnt.UseCases;
using TaskManagemnt.UseCases.Interfaces;
using TaskManagemnt.UseCases.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TaskDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<TaskService>();
builder.Services.AddScoped<IValidator<TaskItem>,TaskValidator>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();