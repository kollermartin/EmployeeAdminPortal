namespace EmployeeAdminPortal.Models.Dto;

public record EmployeeDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Phone { get; set; }
    public string? Email { get; set; }
    public decimal Salary { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}