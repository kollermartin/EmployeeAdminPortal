using EmployeeAdminPortal.Data;
using EmployeeAdminPortal.Logger;
using EmployeeAdminPortal.Mappers;
using EmployeeAdminPortal.Models.Dto;
using EmployeeAdminPortal.Models.Entities;
using EmployeeAdminPortal.Models.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeAdminPortal.Controllers;

[ApiController]
[Route("api/employees")]
public class EmployeesController : ControllerBase
{
    //global error handling
    //https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling?view=aspnetcore-9.0
    //https://learn.microsoft.com/en-us/ef/core/modeling/keys?tabs=fluent-api
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<EmployeesController> _logger;
    private readonly IValidator<AddEmployeeDto> _addEmployeeValidator;
    private readonly IValidator<UpdateEmployeeDto> _updateEmployeeValidator;
    
    public EmployeesController(ApplicationDbContext dbContext, ILogger<EmployeesController> logger, IValidator<AddEmployeeDto> addEmployeeValidator, IValidator<UpdateEmployeeDto> updateEmployeeValidator)
    {
        _dbContext = dbContext;
        _logger = logger;
        _addEmployeeValidator = addEmployeeValidator;
        _updateEmployeeValidator = updateEmployeeValidator;
    }
    
    [HttpGet]
    public async Task<Ok<List<EmployeeDto>>> GetAllEmployees()
    {
        _logger.LogInformation(EmployeesControllerLogEvents.ListEmployees,"Fetching all employees from the database");
        var allEmployees = await _dbContext.Employees.Select(e => EmployeeMapper.ConvertToDto(e)).ToListAsync();
        _logger.LogInformation(EmployeesControllerLogEvents.ListEmployees,"Fetched all employees {Count} from the database", allEmployees.Count);
        
        return TypedResults.Ok(allEmployees);
    }

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<Results<NotFound, Ok<EmployeeDto>>> GetEmployeeById(Guid id)
    {
        _logger.LogInformation(EmployeesControllerLogEvents.GetEmployee,"Fetching employee with id {Id} from the database", id);
        var employee = await _dbContext.Employees.Where(e => e.Id == id).Select(e => EmployeeMapper.ConvertToDto(e)).FirstOrDefaultAsync();
        if (employee is null)
        {
            _logger.LogWarning(EmployeesControllerLogEvents.GetEmployee,"Employee with id {Id} not found in the database", id);
            return TypedResults.NotFound();
        }
        
        _logger.LogInformation(EmployeesControllerLogEvents.GetEmployee,"Fetched employee with id {Id} from the database", id);
        return TypedResults.Ok(employee);
    }

    [HttpPost]
    public async Task<Results<ValidationProblem, Ok<EmployeeDto>>> AddEmployee(AddEmployeeDto addEmployeeDto)
    {
        _logger.LogInformation(EmployeesControllerLogEvents.AddEmployee,"Adding employee to the database");
        
        var validationResult = await _addEmployeeValidator.ValidateAsync(addEmployeeDto);
        
        if (!validationResult.IsValid)
        {
            _logger.LogWarning(EmployeesControllerLogEvents.AddEmployee,"Validation failed for new employee: {ValidationErrors}", validationResult.Errors);
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

        _logger.LogInformation(EmployeesControllerLogEvents.AddEmployee,"Added employee with id {Id} to the database", employeeEntity.Id);
        return TypedResults.Ok(employeeDto);
    }

    [HttpPut]
    [Route("{id:guid}")]
    public async Task<Results<NotFound, ValidationProblem, Ok<EmployeeDto>>> UpdateEmployee(Guid id, UpdateEmployeeDto updateEmployeeDto)
    {
        _logger.LogInformation(EmployeesControllerLogEvents.UpdateEmployee,"Updating employee with id {Id} in the database", id);
        
        var validationResult = await _updateEmployeeValidator.ValidateAsync(updateEmployeeDto);
        
        // Divná error message když je salary jako string
        if (!validationResult.IsValid)
        {
            _logger.LogWarning(EmployeesControllerLogEvents.UpdateEmployee,"Validation failed for updated employee: {ValidationErrors}", validationResult.Errors);
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }
        
        var employee = await _dbContext.Employees.FirstOrDefaultAsync(em => em.Id == id);

        if (employee is null)
        {
            _logger.LogWarning(EmployeesControllerLogEvents.UpdateEmployee,"Employee with id {Id} not found in the database", id);
            return TypedResults.NotFound();
        }

        employee.Name = updateEmployeeDto.Name ?? employee.Name;
        employee.Phone = updateEmployeeDto.Phone ?? employee.Phone;
        employee.Email = updateEmployeeDto.Email ?? employee.Email;
        employee.Salary = updateEmployeeDto.Salary ?? employee.Salary;

        await _dbContext.SaveChangesAsync();

        var employeeDto = EmployeeMapper.ConvertToDto(employee);
        
        _logger.LogInformation(EmployeesControllerLogEvents.UpdateEmployee,"Updated employee with id {Id} in the database", id);
        return TypedResults.Ok(employeeDto);
    }

    [HttpDelete]
    [Route("{id:guid}")]
    public async Task<Results<NotFound, Ok<EmployeeDto>>> DeleteEmployee(Guid id)
    {
        _logger.LogInformation(EmployeesControllerLogEvents.DeleteEmployee,"Deleting employee with id {Id} from the database", id);
        var employee = await _dbContext.Employees.FirstOrDefaultAsync(em => em.Id == id);

        if (employee is null)
        {
            _logger.LogWarning("Employee with id {Id} not found in the database", id);
            return TypedResults.NotFound();
        }
        
        _dbContext.Employees.Remove(employee);
        await _dbContext.SaveChangesAsync();
        
        var employeeDto = EmployeeMapper.ConvertToDto(employee);
        
        _logger.LogInformation(EmployeesControllerLogEvents.DeleteEmployee,"Deleted employee with id {Id} from the database", id);
        return TypedResults.Ok(employeeDto);
    }
}