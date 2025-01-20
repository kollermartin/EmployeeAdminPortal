using EmployeeAdminPortal.Models.Dto;
using FluentValidation;

namespace EmployeeAdminPortal.Models.Validators;

public class UpdateEmployeeValidator : AbstractValidator<UpdateEmployeeDto>
{
    public UpdateEmployeeValidator()
    {
        RuleFor(employee => employee.Name)
            .MaximumLength(50);
        RuleFor(employee => employee.Email).EmailAddress();
        RuleFor(employee => employee.Salary).GreaterThanOrEqualTo(0);
    }
}