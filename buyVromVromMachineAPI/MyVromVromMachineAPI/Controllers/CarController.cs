using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyVromVromMachineAPI.Data;

namespace MyVromVromMachineAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CarController : ControllerBase
{
    private readonly DataContext _context;

    public CarController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? brand, [FromQuery] decimal? maxPrice)
    {
        var query = _context.Cars.Where(c => c.IsAvailable);

        if (!string.IsNullOrWhiteSpace(brand)) query = query.Where(c => c.Brand == brand);
        if (maxPrice.HasValue) query = query.Where(c => c.Price <= maxPrice.Value);

        return Ok(await query.ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var car = await _context.Cars.FindAsync(id);
        if (car == null) return NotFound();
        return Ok(car);
    }
}