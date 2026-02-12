namespace Core.Helpers;
using AutoMapper;
using Core.DTOs;
using Core.Entities;    

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDTO>();
    }
}
