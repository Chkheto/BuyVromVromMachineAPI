using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyVromVromMachineAPI.Data;
using MyVromVromMachineAPI.DTO.Requests.Cars;
using MyVromVromMachineAPI.Services.Cars;
using MyVromVromMachineAPI.models;
using MyVromVromMachineAPI.Services.UserService;
using AppUser = MyVromVromMachineAPI.models.User;

namespace MyVromVromMachineAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CarController : ControllerBase
{
    private readonly DataContext _context;
    private readonly ICarService _carService;
    private readonly IUserService _users;
    private readonly IValidator<AddCar> _validator;

    public CarController(DataContext context, ICarService carService, IUserService users, IValidator<AddCar> validator)
    {
        _context = context;
        _carService = carService;
        _users = users;
        _validator = validator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? brand,
        [FromQuery] string? model,
        [FromQuery] string? fuelType,
        [FromQuery] string? transmission,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] int? minYear,
        [FromQuery] int? maxYear,
        [FromQuery] int? maxMileage,
        [FromQuery] string? sortBy,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = _context.Cars.AsNoTracking().Where(c => c.IsAvailable);

        if (!string.IsNullOrWhiteSpace(brand)) query = query.Where(c => c.Brand == brand);
        if (!string.IsNullOrWhiteSpace(model)) query = query.Where(c => c.Model.Contains(model));
        if (!string.IsNullOrWhiteSpace(fuelType)) query = query.Where(c => c.FuelType == fuelType);
        if (!string.IsNullOrWhiteSpace(transmission)) query = query.Where(c => c.Transmission == transmission);
        if (minPrice.HasValue) query = query.Where(c => c.Price >= minPrice.Value);
        if (maxPrice.HasValue) query = query.Where(c => c.Price <= maxPrice.Value);
        if (minYear.HasValue) query = query.Where(c => c.Year >= minYear.Value);
        if (maxYear.HasValue) query = query.Where(c => c.Year <= maxYear.Value);
        if (maxMileage.HasValue) query = query.Where(c => c.Mileage <= maxMileage.Value);

        query = sortBy switch
        {
            "price_asc" => query.OrderBy(c => c.Price),
            "price_desc" => query.OrderByDescending(c => c.Price),
            "year_desc" => query.OrderByDescending(c => c.Year),
            "mileage_asc" => query.OrderBy(c => c.Mileage),
            _ => query.OrderBy(c => c.Brand).ThenBy(c => c.Model)
        };

        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 50);

        var total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return Ok(new { total, page, pageSize, items });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var car = await _context.Cars.FindAsync(id);
        if (car == null) return NotFound();
        return Ok(car);
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Add([FromForm] AddCar request)
    {
        var result = await _validator.ValidateAsync(request);
        if (!result.IsValid) return BadRequest(result.Errors);

        var user = Authenticate(request.SellerEmail, request.SellerPassword);
        if (user == null) return Unauthorized("invalid email or password");

        if (user.Role != Roles.Seller && !user.IsAdmin)
            return StatusCode(StatusCodes.Status403Forbidden, "only sellers and admins can add cars");

        await _carService.AddCar(request, user.Id);
        return Ok("car added successfully");
    }

    [HttpPost("{id:guid}/remove")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remove(Guid id, [FromBody] RemoveCarRequest req)
    {
        var user = Authenticate(req.Email, req.Password);
        if (user == null) return Unauthorized("invalid email or password");

        var car = await _context.Cars.FindAsync(id);
        if (car == null) return NotFound();

        if (!CanManage(user, car))
            return StatusCode(StatusCodes.Status403Forbidden, "only an admin or the seller of this car can remove it");

        DeleteImageFile(car.ImageUrl);

        _context.Cars.Remove(car);
        await _context.SaveChangesAsync();

        return Ok("car removed successfully");
    }

    [HttpPatch("{id:guid}/availability")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetAvailability(Guid id, [FromBody] SetAvailabilityRequest req)
    {
        var user = Authenticate(req.Email, req.Password);
        if (user == null) return Unauthorized("invalid email or password");

        var car = await _context.Cars.FindAsync(id);
        if (car == null) return NotFound();

        if (!CanManage(user, car))
            return StatusCode(StatusCodes.Status403Forbidden, "only an admin or the seller of this car can change it");

        car.IsAvailable = req.IsAvailable;
        await _context.SaveChangesAsync();

        return Ok(req.IsAvailable ? "car is listed again" : "car removed from the listing");
    }

    private AppUser? Authenticate(string? email, string? password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrEmpty(password)) return null;

        var user = _users.Login(email, password);
        return user != null && user.IsVerified ? user : null;
    }

    private static bool CanManage(AppUser user, Car car) =>
        user.IsAdmin || (user.Role == Roles.Seller && car.SellerId == user.Id);

    private static void DeleteImageFile(string? imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl) || !imageUrl.StartsWith("/uploads/")) return;

        var uploadsRoot = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "uploads")) + Path.DirectorySeparatorChar;
        var relative = imageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), relative));

        if (fullPath.StartsWith(uploadsRoot) && System.IO.File.Exists(fullPath))
            System.IO.File.Delete(fullPath);
    }
}