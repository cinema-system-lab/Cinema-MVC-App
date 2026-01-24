using AutoMapper;
using Core.DTOs;
using Core.Entities;

namespace Core.Helpers;

public class SeatProfile: Profile
{
    public SeatProfile()
    {
        CreateMap<Seat, SeatDTO>().ReverseMap();
    }
}