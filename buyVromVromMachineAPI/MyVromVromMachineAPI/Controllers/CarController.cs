using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyVromVromMachineAPI.Data;
using MyVromVromMachineAPI.DTO.Requests.Cars;
using MyVromVromMachineAPI.Services.Cars;

namespace MyVromVromMachineAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CarController : ControllerBase
{
    private readonly DataContext _context;
    private readonly ICarService _carService;
    private readonly IValidator<AddCar> _validator;

    public CarController(DataContext context, ICarService carService, IValidator<AddCar> validator)
    {
        _context = context;
        _carService = carService;
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
    public async Task<IActionResult> Add([FromForm] AddCar request)
    {
        var result = await _validator.ValidateAsync(request);
        if (!result.IsValid) return BadRequest(result.Errors);

        await _carService.AddCar(request);
        return Ok("car added successfully");
    }
}