using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MyVromVromMachineAPI.DTO.Request;
using MyVromVromMachineAPI.Services.UserService;

namespace WebApplication1.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _us;
    private readonly IValidator<RegisterRequest> _validator;

    public UserController(IUserService us, IValidator<RegisterRequest> validator)
    {
        _us = us;
        _validator = validator;
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest req)
    {
        var result = _validator.Validate(req);
        if (!result.IsValid) return BadRequest(result.Errors);

        var usertoadd = _us.Register(req);

        return Ok("user created successfully");
    }

    [HttpDelete("delete-user/{id}")]
    public IActionResult Delete(int id)
    {
        var deleted = _us.DeleteUser(id);
        return Ok("user deleted successfully");
    }

    [HttpPut("update-user/{id}")]
    public IActionResult Update(int id, [FromBody] EditUserRequest req)
    {
        var updated = _us.UpdateUser(id, req);
        return Ok("user updated successfully");
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest req)
    {
        var result = _us.Login(req.Email, req.Password);
        return Ok();
    }
}