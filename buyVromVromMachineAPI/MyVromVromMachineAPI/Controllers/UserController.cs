using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MyVromVromMachineAPI.DTO.Request;
using MyVromVromMachineAPI.DTO.Requests;
using MyVromVromMachineAPI.DTO.Requests.User;
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

        if (!_us.Register(req)) return Conflict("email already registered");
        return Ok("user created successfully");
    }

    [HttpDelete("delete-user/{id}")]
    public IActionResult Delete(int id)
    {
        if (!_us.DeleteUser(id)) return NotFound();
        return Ok("user deleted successfully");
    }

    [HttpPut("update-user/{id}")]
    public IActionResult Update(int id, [FromBody] EditUserRequest req)
    {
        var updated = _us.UpdateUser(id, req);
        if (!_us.UpdateUser(id, req)) return NotFound();
        return Ok("user updated successfully");
    }
    [HttpPost("verify")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Verify([FromBody] VerifyRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Email) || req.Code.Length != 6)
            return BadRequest("email and a 6-digit code are required");

        if (!_us.Verify(req.Email, req.Code))
            return BadRequest("invalid or expired code");

        return Ok("email verified successfully");
    }

    [HttpPost("resend-code")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult ResendCode([FromBody] ResendCodeRequest req)
    {
        if (!_us.ResendCode(req.Email))
            return BadRequest("cannot send a new code right now");

        return Ok("a new code was sent");
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult Login([FromBody] LoginRequest req)
    {
        var user = _us.Login(req.Email, req.Password);
        if (user == null) return Unauthorized("invalid email or password");
        if (!user.IsVerified) return StatusCode(StatusCodes.Status403Forbidden, "email not verified");

        return Ok(new { user.Id, user.Fullname, user.Email });
    }
}