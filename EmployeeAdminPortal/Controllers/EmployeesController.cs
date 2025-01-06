using EmployeeAdminPortal.Data;
using EmployeeAdminPortal.Models;
using EmployeeAdminPortal.Models.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeAdminPortal.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    //TODO FluentValidation
    //TODO mapovat na dto přes static metodu
    //TODO Nahradit IactionRestult za Result
    //TODO Použít recordType místo classy pro DTO
    //TODO Async / await
    //TODO nahradit Find
    //https://learn.microsoft.com/en-us/ef/core/modeling/keys?tabs=fluent-api
    private readonly ApplicationDbContext _dbContext;
    public EmployeesController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    [HttpGet]
    public IActionResult GetAllEmployees()
    {
        var allEmployees = _dbContext.Employees.ToList();
        
        return Ok(allEmployees);
    }

    [HttpGet]
    [Route("{id:guid}")]
    public Results<NotFound, Ok<Employee>> GetEmployeeById(Guid id)
    {
        var employee = _dbContext.Employees.Find(id);
        if (employee is null)
        {
            return TypedResults.NotFound();
        }
        
        return TypedResults.Ok(employee);
    }

    [HttpPost]
    public IActionResult AddEmployee(AddEmployeeDto addEmployeeDto)
    {
        var employeeEntity = new Employee()
        {
            Name = addEmployeeDto.Name,
            Phone = addEmployeeDto.Phone,
            Email = addEmployeeDto.Email,
            Salary = addEmployeeDto.Salary,
        };
        
        _dbContext.Employees.Add(employeeEntity);
        _dbContext.SaveChanges();

        return Ok(employeeEntity);
    }

    [HttpPut]
    [Route("{id:guid}")]
    public IActionResult UpdateEmployee(Guid id, UpdateEmployeeDto updateEmployeeDto)
    {
        var employee = _dbContext.Employees.Find(id);

        if (employee is null)
        {
            return NotFound();
        }

        employee.Name = updateEmployeeDto.Name ?? employee.Name;
        employee.Phone = updateEmployeeDto.Phone ?? employee.Phone;
        employee.Email = updateEmployeeDto.Email ?? employee.Email;
        employee.Salary = updateEmployeeDto.Salary ?? employee.Salary;

        _dbContext.SaveChanges();
        
        return Ok(employee);
    }

    [HttpDelete]
    [Route("{id:guid}")]
    public IActionResult DeleteEmployee(Guid id)
    {
        var employee = _dbContext.Employees.Find(id);

        if (employee is null)
        {
            return NotFound();
        }
        
        _dbContext.Employees.Remove(employee);
        _dbContext.SaveChanges();
        
        return Ok(employee);
    }
}