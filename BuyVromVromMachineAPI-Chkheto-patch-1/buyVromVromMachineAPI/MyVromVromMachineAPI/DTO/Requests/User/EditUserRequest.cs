namespace MyVromVromMachineAPI.DTO.Requests.User;

public class EditUserRequest
{
    public string Email { get; set; } = string.Empty;
    public string? CurrentPassword { get; set; }
    public string? Code { get; set; }
    public string? Fullname { get; set; }
    public string? NewPassword { get; set; }
    public int? Age { get; set; }
}