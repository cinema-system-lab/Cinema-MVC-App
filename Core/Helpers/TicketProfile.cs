using AutoMapper;
using Core.DTOs;
using Core.Entities;
using Core.Enums;

namespace Core.Helpers;

public class TicketProfile : Profile
{
    public TicketProfile()
    {
        CreateMap<Ticket, TicketDTO>()
            .ForMember(d => d.MovieTitle, opt => opt.MapFrom(s => s.Session.Movie.Title))
            .ForMember(d => d.HallName, opt => opt.MapFrom(s => s.Session.Hall.Name))
            .ForMember(d => d.StartTime, opt => opt.MapFrom(s => s.Session.StartTime))
            .ForMember(d => d.SeatType, opt => opt.MapFrom(s => s.Seat.Type))
            .ForMember(d => d.Price, opt => opt.MapFrom(s => 
                s.Seat.Type == SeatType.Premium 
                    ? s.Session.BasePrice * 1.5m 
                    : s.Session.BasePrice))
            .ForMember(d => d.RowNumber, opt => opt.MapFrom(s => s.Seat.RowNumber))
            .ForMember(d => d.SeatNumber, opt => opt.MapFrom(s => s.Seat.SeatNumber));

        CreateMap<TicketCreateDTO, Ticket>();
    }
}