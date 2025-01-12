using EmployeeAdminPortal.Models.Dto;
using EmployeeAdminPortal.Models.Entities;

namespace EmployeeAdminPortal.Mappers;

public static class EmployeeMapper
{
    public static EmployeeDto ConvertToDto(Employee employee)
    {
        return new EmployeeDto()
        {
            Id = employee.Id,
            Name = employee.Name,
            Phone = employee.Phone,
            Email = employee.Email,
            Salary = employee.Salary
        };
    }
}