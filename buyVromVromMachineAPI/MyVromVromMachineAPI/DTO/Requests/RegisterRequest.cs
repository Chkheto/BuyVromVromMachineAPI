namespace MyVromVromMachineAPI.DTO.Request;

public class RegisterRequest
{
    public string Fullname { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string PhoneNumber { get; set; } = string.Empty; 
    public int Age { get; set; }
}
