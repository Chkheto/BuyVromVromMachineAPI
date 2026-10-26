namespace MyVromVromMachineAPI.DTO.Requests.Cars;

public class RemoveCarRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class SetAvailabilityRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
}