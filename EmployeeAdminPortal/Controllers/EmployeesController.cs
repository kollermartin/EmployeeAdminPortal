using EmployeeAdminPortal.Data;
using EmployeeAdminPortal.Mappers;
using EmployeeAdminPortal.Models.Dto;
using EmployeeAdminPortal.Models.Entities;
using EmployeeAdminPortal.Models.Validators;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeAdminPortal.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    //TODO logging
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
    public async Task<Results<NotFound, Ok<EmployeeDto>>> GetEmployeeById(Guid id)
    {
        var employee = await _dbContext.Employees.FirstOrDefaultAsync(em => em.Id == id);
        if (employee is null)
        {
            return TypedResults.NotFound();
        }
        
        return TypedResults.Ok(EmployeeMapper.ConvertToDto(employee));
    }

    [HttpPost]
    public async Task<Results<BadRequest, ValidationProblem, Ok<EmployeeDto>>> AddEmployee(AddEmployeeDto addEmployeeDto)
    {
        var validator = new AddEmployeeValidator();
        var validationResult = await validator.ValidateAsync(addEmployeeDto);
        
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }
        
        var employeeEntity = new Employee()
        {
            Name = addEmployeeDto.Name,
            Phone = addEmployeeDto.Phone,
            Email = addEmployeeDto.Email,
            Salary = addEmployeeDto.Salary,
        };
        
        await _dbContext.Employees.AddAsync(employeeEntity);
        await _dbContext.SaveChangesAsync();

        var employeeDto = EmployeeMapper.ConvertToDto(employeeEntity);

        return TypedResults.Ok(employeeDto);
    }

    [HttpPut]
    [Route("{id:guid}")]
    public async Task<Results<NotFound, ValidationProblem, Ok<EmployeeDto>>> UpdateEmployee(Guid id, UpdateEmployeeDto updateEmployeeDto)
    {
        var validator = new UpdateEmployeeValidator();
        var validationResult = await validator.ValidateAsync(updateEmployeeDto);
        
        // Divná error message když je salary jako string
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }
        
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

        var employeeDto = EmployeeMapper.ConvertToDto(employee);
        
        return TypedResults.Ok(employeeDto);
    }

    [HttpDelete]
    [Route("{id:guid}")]
    public async Task<Results<NotFound, Ok<EmployeeDto>>> DeleteEmployee(Guid id)
    {
        var employee = await _dbContext.Employees.FirstOrDefaultAsync(em => em.Id == id);

        if (employee is null)
        {
            return TypedResults.NotFound();
        }
        
        _dbContext.Employees.Remove(employee);
        await _dbContext.SaveChangesAsync();
        
        var employeeDto = EmployeeMapper.ConvertToDto(employee);
        
        return TypedResults.Ok(employeeDto);
    }
}