using MyVromVromMachineAPI.DTO.Requests.Cars;

namespace MyVromVromMachineAPI.Services.Cars;

public interface ICarService
{
    Task<bool> AddCar(AddCar car);
}
