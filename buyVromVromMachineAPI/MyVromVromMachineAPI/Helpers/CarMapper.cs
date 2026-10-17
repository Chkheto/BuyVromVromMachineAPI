using AutoMapper;
using MyVromVromMachineAPI.DTO.Requests.Cars;
using MyVromVromMachineAPI.models;

namespace MyVromVromMachineAPI.Helpers;

public class CarMapper : Profile
{
    public CarMapper()
    {
        CreateMap<Car, AddCar>().ReverseMap();
    }
}
