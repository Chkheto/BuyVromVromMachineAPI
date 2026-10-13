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

    public UserController(IUserService us) => _us = us;

    [HttpPost("register")]
    public IActionResult Register(RegisterRequest req, [FromServices] IValidator<RegisterRequest> validator)
    {
        var result = validator.Validate(req);
        if (!result.IsValid) return BadRequest(result);

        var usertoadd = _us.Register(req);

        return Ok("user created successfully");
    }

    [HttpDelete("delete-user/{id}")] // Fixed route syntax from ${id} to {id}
    public IActionResult Delete(int id)
    {
        var deleted = _us.DeleteUser(id);
        return Ok("user deleted succesfully");
    }

    [HttpPut("update-user/{id}")] // Fixed route syntax from ${id} to {id}
    public IActionResult Update(int id, EditUserRequest req)
    {
        var updated = _us.UpdateUser(id, req);
        return Ok("user updated succesfully");
    }

    [HttpPost("login")]
    public IActionResult Login(string email, string password)
    {
        var result = _us.Login(email, password);
        return Ok();
    }
}