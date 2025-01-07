using EmployeeAdminPortal.Data;
using EmployeeAdminPortal.Models;
using EmployeeAdminPortal.Models.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeAdminPortal.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    //TODO FluentValidation
    //TODO mapovat na dto přes static metodu
    //https://learn.microsoft.com/en-us/ef/core/modeling/keys?tabs=fluent-api
    private readonly ApplicationDbContext _dbContext;
    public EmployeesController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    [HttpGet]
    public async Task<IResult> GetAllEmployees()
    {
        var allEmployees = await _dbContext.Employees.ToListAsync();
        
        return Results.Ok(allEmployees);
    }

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<Results<NotFound, Ok<Employee>>> GetEmployeeById(Guid id)
    {
        var employee = await _dbContext.Employees.FirstOrDefaultAsync(em => em.Id == id);
        if (employee is null)
        {
            return TypedResults.NotFound();
        }
        
        return TypedResults.Ok(employee);
    }

    [HttpPost]
    public async Task<IResult> AddEmployee(AddEmployeeDto addEmployeeDto)
    {
        var employeeEntity = new Employee()
        {
            Name = addEmployeeDto.Name,
            Phone = addEmployeeDto.Phone,
            Email = addEmployeeDto.Email,
            Salary = addEmployeeDto.Salary,
        };
        
        await _dbContext.Employees.AddAsync(employeeEntity);
        await _dbContext.SaveChangesAsync();

        return Results.Ok(employeeEntity);
    }

    [HttpPut]
    [Route("{id:guid}")]
    public async Task<Results<NotFound, Ok<Employee>>> UpdateEmployee(Guid id, UpdateEmployeeDto updateEmployeeDto)
    {
        var employee = await _dbContext.Employees.FirstOrDefaultAsync(em => em.Id == id);

        if (employee is null)
        {
            return TypedResults.NotFound();
        }

        employee.Name = updateEmployeeDto.Name ?? employee.Name;
        employee.Phone = updateEmployeeDto.Phone ?? employee.Phone;
        employee.Email = updateEmployeeDto.Email ?? employee.Email;
        employee.Salary = updateEmployeeDto.Salary ?? employee.Salary;

        await _dbContext.SaveChangesAsync();
        
        return TypedResults.Ok(employee);
    }

    [HttpDelete]
    [Route("{id:guid}")]
    public async Task<Results<NotFound, Ok<Employee>>> DeleteEmployee(Guid id)
    {
        var employee = await _dbContext.Employees.FirstOrDefaultAsync(em => em.Id == id);

        if (employee is null)
        {
            return TypedResults.NotFound();
        }
        
        _dbContext.Employees.Remove(employee);
        await _dbContext.SaveChangesAsync();
        
        return TypedResults.Ok(employee);
    }
}