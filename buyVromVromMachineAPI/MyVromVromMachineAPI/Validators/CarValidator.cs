using FluentValidation;
using MyVromVromMachineAPI.DTO.Requests.Cars;

namespace MyVromVromMachineAPI.Validators;

public class CarValidator : AbstractValidator<AddCar>
{
    private static readonly string[] FuelTypes = ["Petrol", "Diesel", "Electric", "Hybrid"];
    private static readonly string[] Transmissions = ["Manual", "Automatic"];
    private static readonly string[] ImageExtensions = [".jpg", ".jpeg", ".png", ".webp"];

    public CarValidator()
    {
        RuleFor(x => x.Brand).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Model).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Year).InclusiveBetween(1950, DateTime.UtcNow.Year + 1);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.Mileage).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Color).NotEmpty().MaximumLength(30);

        RuleFor(x => x.FuelType)
            .Must(f => FuelTypes.Contains(f, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Fuel type must be Petrol, Diesel, Electric or Hybrid");

        RuleFor(x => x.Transmission)
            .Must(t => Transmissions.Contains(t, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Transmission must be Manual or Automatic");

        RuleFor(x => x.ImageUrl)
            .Must(f => f == null || f.Length <= 5 * 1024 * 1024)
            .WithMessage("Image must be 5 MB or smaller")
            .Must(f => f == null || ImageExtensions.Contains(Path.GetExtension(f.FileName).ToLowerInvariant()))
            .WithMessage("Image must be jpg, png or webp");
    }
}