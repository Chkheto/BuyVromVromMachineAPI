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

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required")
            .Matches(@"^\+?[0-9]{9,15}$").WithMessage("Phone number format is invalid");

        RuleFor(x => x.Age)
            .InclusiveBetween(18, 120).WithMessage("You must be at least 18 years old");
    }
}

public class EditUserValidator : AbstractValidator<EditUserRequest>
{
    public EditUserValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();

        RuleFor(x => x)
            .Must(x => !string.IsNullOrEmpty(x.CurrentPassword) || !string.IsNullOrEmpty(x.Code))
            .WithMessage("Provide your current password or a verification code");

        RuleFor(x => x.Fullname)
            .MinimumLength(2).MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.Fullname));

        RuleFor(x => x.NewPassword)
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .Matches("[A-Z]").WithMessage("Password needs an uppercase letter")
            .Matches("[a-z]").WithMessage("Password needs a lowercase letter")
            .Matches("[0-9]").WithMessage("Password needs a digit")
            .When(x => !string.IsNullOrEmpty(x.NewPassword));

        RuleFor(x => x.Age)
            .InclusiveBetween(18, 120)
            .When(x => x.Age.HasValue);
    }
}

public class DeleteUserValidator : AbstractValidator<DeleteUserRequest>
{
    public DeleteUserValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
        RuleFor(x => x.Code).Matches(@"^[0-9]{6}$").WithMessage("Code must be 6 digits");
    }
}