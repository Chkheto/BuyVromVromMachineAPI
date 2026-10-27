namespace MyVromVromMachineAPI.models;

public class User
{
    public int Id { get; set; }
    public string Fullname { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
    public int Age { get; set; }
    public bool IsVerified { get; set; } = true;
    public string? VerificationCode { get; set; }
    public DateTime? VerificationExpiry { get; set; }
    public int VerificationAttempts { get; set; }
    public string Role { get; set; } = Roles.Buyer;
    public bool IsAdmin => Roles.IsAdminEmail(Email);

    public User() { }

    public User(int id, string fullname, string email, string phoneNumber, string password, int age, bool isVerified)
    {
        Id = id;
        Fullname = fullname;
        Email = email;
        PhoneNumber = phoneNumber;
        Password = password;
        Age = age;
        IsVerified = isVerified;
    }
}