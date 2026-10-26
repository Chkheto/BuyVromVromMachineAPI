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
    private readonly IValidator<EditUserRequest> _editValidator;
    private readonly IValidator<DeleteUserRequest> _deleteValidator;

    public UserController(
        IUserService us,
        IValidator<RegisterRequest> validator,
        IValidator<EditUserRequest> editValidator,
        IValidator<DeleteUserRequest> deleteValidator)
    {
        _us = us;
        _validator = validator;
        _editValidator = editValidator;
        _deleteValidator = deleteValidator;
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public IActionResult Register([FromBody] RegisterRequest req)
    {
        var result = _validator.Validate(req);
        if (!result.IsValid) return BadRequest(result.Errors);

        if (!_us.Register(req)) return Conflict("email already registered");
        return Ok("user created successfully");
    }

    [HttpPost("request-code")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult RequestCode([FromBody] RequestCodeRequest req)
    {
        if (!string.IsNullOrWhiteSpace(req.Email)) _us.RequestCode(req.Email);
        return Ok("if the account exists, a code was sent to its email");
    }

    [HttpPost("delete-user")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Delete([FromBody] DeleteUserRequest req)
    {
        var result = _deleteValidator.Validate(req);
        if (!result.IsValid) return BadRequest(result.Errors);

        if (!_us.DeleteUser(req.Email, req.Password, req.Code))
            return Unauthorized("invalid email, password or code");

        return Ok("user deleted successfully");
    }

    [HttpPut("update-user")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Update([FromBody] EditUserRequest req)
    {
        var result = _editValidator.Validate(req);
        if (!result.IsValid) return BadRequest(result.Errors);

        if (!_us.UpdateUser(req)) return Unauthorized("invalid email, password or code");

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