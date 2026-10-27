using AutoMapper;
using MyVromVromMachineAPI.DTO.Request;
using MyVromVromMachineAPI.DTO.Requests.User;
using MyVromVromMachineAPI.models;

namespace MyVromVromMachineAPI.helpers;

public class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<User, RegisterRequest>().ReverseMap();
        CreateMap<User, EditUserRequest>().ReverseMap();
    }

}
