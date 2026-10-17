using MyVromVromMachineAPI.DTO.Request;
using MyVromVromMachineAPI.DTO.Requests.User;
using MyVromVromMachineAPI.models;

namespace MyVromVromMachineAPI.Services.UserService;

public interface IUserService
{
    public bool Register(RegisterRequest req);
    public User? Login(string email, string password);
    public bool Verify(string email, string code);
    public bool ResendCode(string email);
    public bool DeleteUser(int id);
    public bool UpdateUser(int id, EditUserRequest req);
}