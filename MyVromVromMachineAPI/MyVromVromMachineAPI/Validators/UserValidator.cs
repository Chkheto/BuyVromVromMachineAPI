using FluentValidation;
using MyVromVromMachineAPI.models;

namespace MyVromVromMachineAPI.Validators;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        // 1. Email validation: must be a valid email format (using a regex for more specificity) and not empty
        RuleFor(u => u.Email)
            .Matches(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")
            .WithMessage("The Email should be a valid email address.")
            .NotEmpty().WithMessage("Email is required.");

        // 2. Password validation: must be between 6 and 30 characters, not empty, and contain at least one uppercase letter, one lowercase letter, one number, and one special character.
        // if diffrent regex needed, feel free to change up
        RuleFor(u => u.Password)
            .Length(6,30)
            .WithMessage("Password must be in between 6 and 30 characters.")
            // if diffrent regex needed, feel free to change up
            .NotEmpty().WithMessage("Password is required.").Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,30}$")
            .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character (@$!%*?&).");
    }
}