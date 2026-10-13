using AutoMapper;
using MyVromVromMachineAPI.Data;
using MyVromVromMachineAPI.DTO.Request;
using MyVromVromMachineAPI.models;
using MyVromVromMachineAPI.SMTP;

namespace MyVromVromMachineAPI.Services.UserService;

public class UsersService : IUserService
{
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

        var mail = SMTPService.SendEmail(req.Email, req.Fullname);

        return true;
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
        var userExists = _context.Users.FirstOrDefault(u => u.Id == id);
        if (userExists == null) return false;

        var mapped = _mapper.Map<User>(req);
        _context.Users.Update(mapped);
        _context.SaveChanges();

        return true;
    }

    public User Login(string email, string password)
    {
        var userexists = _context.Users.FirstOrDefault(u => u.Email == email &&
        password == u.Password);

        if (userexists == null) return null;
        if (!BCrypt.Net.BCrypt.Verify(password, userexists.Password)) return null;

        return userexists;


    }

    public bool Verify(string email, string code)
    {
        throw new NotImplementedException();
    }
}
