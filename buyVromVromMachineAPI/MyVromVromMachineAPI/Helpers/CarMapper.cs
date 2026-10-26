using AutoMapper;
using MyVromVromMachineAPI.DTO.Requests.Cars;
using MyVromVromMachineAPI.models;

namespace MyVromVromMachineAPI.Helpers;

public class CarMapper : Profile
{
    public CarMapper()
    {
        CreateMap<AddCar, Car>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.ImageUrl, o => o.Ignore())
            .ForMember(d => d.IsAvailable, o => o.Ignore())
            .ForMember(d => d.SellerId, o => o.Ignore());
    }
}