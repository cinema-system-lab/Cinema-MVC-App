using AutoMapper;
using Core.DTOs;
using Core.Entities;

namespace Core.Helpers;

public class HallProfile : Profile
{
    public HallProfile()
    {
        CreateMap<Hall, HallDTO>().ReverseMap();
    }
}
