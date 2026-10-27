namespace MyVromVromMachineAPI.DTO.Requests.User;

public class RequestCodeRequest
{
    public string Email { get; set; } = string.Empty;
}

public class DeleteUserRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}