using AutoMapper;
using Core.DTOs;
using Core.Entities;

namespace Core.Helpers;

public class PaymentProfile : Profile
{
    public PaymentProfile()
    {
        CreateMap<Payment, PaymentDTO>().ReverseMap();
        
        CreateMap<CreatePaymentDTO, Payment>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.PaymentDate, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Core.Enums.PaymentStatus.Pending));
    }
}