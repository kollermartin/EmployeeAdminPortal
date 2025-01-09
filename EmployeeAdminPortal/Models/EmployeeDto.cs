namespace EmployeeAdminPortal.Models;

public record EmployeeDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Phone { get; set; }
    public string? Email { get; set; }
    public decimal Salary { get; set; }
}