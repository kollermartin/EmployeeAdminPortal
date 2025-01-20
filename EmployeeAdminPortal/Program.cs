using EmployeeAdminPortal.Data;
using EmployeeAdminPortal.Models.Dto;
using EmployeeAdminPortal.Models.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IValidator<AddEmployeeDto>, AddEmployeeValidator>();
builder.Services.AddScoped<IValidator<UpdateEmployeeDto>, UpdateEmployeeValidator>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Enables Swagger middleware
    app.UseSwaggerUI(); // Enables Swagger UI middleware
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();