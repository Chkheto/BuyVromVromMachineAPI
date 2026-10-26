namespace MyVromVromMachineAPI.DTO.Requests.Cars;

public class AddCar
{
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal Price { get; set; }
    public int Mileage { get; set; }
    public string FuelType { get; set; } = string.Empty;
    public string Transmission { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string SellerEmail { get; set; } = string.Empty;
    public string SellerPassword { get; set; } = string.Empty;
    public IFormFile? ImageUrl { get; set; }
}
