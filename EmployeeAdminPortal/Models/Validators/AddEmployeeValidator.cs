using EmployeeAdminPortal.Models.Dto;
using FluentValidation;

namespace EmployeeAdminPortal.Models.Validators;

public class AddEmployeeValidator : AbstractValidator<AddEmployeeDto>
{
    public AddEmployeeValidator()
    {
        RuleFor(employee => employee.Name)
            .NotEmpty()
            .MaximumLength(50);
        RuleFor(employee => employee.Email).NotEmpty().EmailAddress();
        RuleFor(employee => employee.Phone).NotEmpty();
        RuleFor(employee => employee.Salary).GreaterThanOrEqualTo(0);
    }
}