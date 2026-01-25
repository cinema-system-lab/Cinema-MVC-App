using AutoMapper;
using Core.DTOs;
using Core.Entities;

namespace Core.Helpers;

public class SessionProfile : Profile
{
    public SessionProfile()
    {
        CreateMap<Session, SessionDTO>().ReverseMap();
    }
}