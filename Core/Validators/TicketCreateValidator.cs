using Core.DTOs;
using FluentValidation;

namespace Core.Validators
{
    public class TicketCreateValidator : AbstractValidator<TicketCreateDTO>
    {
        public TicketCreateValidator()
        {
            RuleFor(x => x.OrderId)
                .NotEmpty().WithMessage("OrderId is required");

            RuleFor(x => x.SessionId)
                .GreaterThan(0).WithMessage("Invalid SessionId");

            RuleFor(x => x.SeatId)
                .GreaterThan(0).WithMessage("Invalid SeatId");
        }
    }
}
