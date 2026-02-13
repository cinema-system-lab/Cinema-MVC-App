using AutoMapper;
using Core.DTOs;
using Core.Entities;

namespace Core.Helpers;

public class MovieProfile : Profile
{
    public MovieProfile()
    {
        CreateMap<Movie, MovieDTO>().ReverseMap();
    }
}