namespace EmployeeAdminPortal.Models.Dto;

public record AddEmployeeDto
{
    public required string Name { get; set; }
    public required string Phone { get; set; }
    public required string Email { get; set; }
    public decimal Salary { get; set; }
}