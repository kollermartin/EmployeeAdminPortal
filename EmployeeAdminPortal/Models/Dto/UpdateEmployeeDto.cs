namespace EmployeeAdminPortal.Models.Dto;

public record UpdateEmployeeDto
{
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public decimal? Salary { get; set; }
}