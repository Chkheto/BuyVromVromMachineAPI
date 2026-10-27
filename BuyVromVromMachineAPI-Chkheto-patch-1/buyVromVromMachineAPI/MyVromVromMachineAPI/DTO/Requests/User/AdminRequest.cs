namespace MyVromVromMachineAPI.DTO.Requests.User;

public class AdminUserRequest
{
    public string AdminEmail { get; set; } = string.Empty;
    public string AdminPassword { get; set; } = string.Empty;
    public int UserId { get; set; }
}