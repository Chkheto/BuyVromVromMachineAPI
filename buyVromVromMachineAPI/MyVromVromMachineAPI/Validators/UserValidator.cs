using FluentValidation;
using MyVromVromMachineAPI.DTO.Request;
using MyVromVromMachineAPI.DTO.Requests.User;

namespace MyVromVromMachineAPI.Validators;

public class UserValidator : AbstractValidator<RegisterRequest>
{
    public UserValidator()
    {
        RuleFor(x => x.Fullname)
            .NotEmpty().WithMessage("Full name is required")
            .MinimumLength(2).WithMessage("Full name must be at least 2 characters")
            .MaximumLength(100).WithMessage("Full name must be at most 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email format is invalid")
            .MaximumLength(254);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .Matches("[A-Z]").WithMessage("Password needs an uppercase letter")
            .Matches("[a-z]").WithMessage("Password needs a lowercase letter")
            .Matches("[0-9]").WithMessage("Password needs a digit");

        RuleFor(x => x.Age)
            .InclusiveBetween(18, 120).WithMessage("You must be at least 18 years old");
    }
}

public class EditUserValidator : AbstractValidator<EditUserRequest>
{
    public EditUserValidator()
    {
        RuleFor(x => x.Fullname).NotEmpty().MinimumLength(2).MaximumLength(100);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        RuleFor(x => x.Age).InclusiveBetween(18, 120);
    }
}