using AutoMapper;
using MyVromVromMachineAPI.Data;
using MyVromVromMachineAPI.DTO.Request;
using MyVromVromMachineAPI.DTO.Requests.User;
using MyVromVromMachineAPI.models;
using MyVromVromMachineAPI.SMTP;

namespace MyVromVromMachineAPI.Services.UserService;

public class UsersService : IUserService
{
    private const int CodeLifetimeMinutes = 5;
    private const int MaxAttempts = 5;
    private const int ResendCooldownSeconds = 60;
    private readonly IMapper _mapper;
    private readonly DataContext _context;

    public UsersService(IMapper mapper, DataContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public bool Register(RegisterRequest req)
    {
        var userExists = _context.Users.FirstOrDefault(u => u.Email == req.Email);
        if (userExists != null) return false;

        req.Password = BCrypt.Net.BCrypt.HashPassword(req.Password);

        var userToCreate = _mapper.Map<User>(req);

        _context.Users.Add(userToCreate);
        _context.SaveChanges();

        IssueCode(userToCreate);

        return true;
    }

    public bool Verify(string email, string code)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == email);
        if (user == null) return false;
        if (user.IsVerified) return true;
        if (user.VerificationCode == null || user.VerificationExpiry == null) return false;
        if (user.VerificationExpiry < DateTime.UtcNow) return false;
        if (user.VerificationAttempts >= MaxAttempts) return false;

        if (!BCrypt.Net.BCrypt.Verify(code, user.VerificationCode))
        {
            user.VerificationAttempts++;
            _context.SaveChanges();
            return false;
        }

        user.IsVerified = true;
        user.VerificationCode = null;
        user.VerificationExpiry = null;
        user.VerificationAttempts = 0;
        _context.SaveChanges();

        return true;
    }

    public bool ResendCode(string email)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == email);
        if (user == null || user.IsVerified) return false;

        if (user.VerificationExpiry.HasValue)
        {
            var issuedAt = user.VerificationExpiry.Value.AddMinutes(-CodeLifetimeMinutes);
            if (issuedAt.AddSeconds(ResendCooldownSeconds) > DateTime.UtcNow) return false;
        }

        IssueCode(user);
        return true;
    }

    private void IssueCode(User user)
    {
        var code = SMTPService.GenerateCode();

        user.VerificationCode = BCrypt.Net.BCrypt.HashPassword(code);
        user.VerificationExpiry = DateTime.UtcNow.AddMinutes(CodeLifetimeMinutes);
        user.VerificationAttempts = 0;
        _context.SaveChanges();

#if DEBUG
        Console.WriteLine($"Verification code for {user.Email}: {code}");
#endif

        try
        {
            SMTPService.SendEmail(user.Email, user.Fullname).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Email failed: {ex.Message}");
        }
    }

    public bool DeleteUser(int id)
    {
        var userExists = _context.Users.FirstOrDefault(u => u.Id == id);
        if (userExists == null) return false;

        _context.Users.Remove(userExists);
        _context.SaveChanges();

        return true;
    }

    public bool UpdateUser(int id, EditUserRequest req)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == id);
        if (user == null) return false;

        user.Fullname = req.Fullname;
        user.Age = req.Age;
        user.Password = BCrypt.Net.BCrypt.HashPassword(req.Password);

        _context.SaveChanges();
        return true;
    }

    public User? Login(string email, string password)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == email);
        if (user == null) return null;
        if (!BCrypt.Net.BCrypt.Verify(password, user.Password)) return null;
        return user;
    }


}
