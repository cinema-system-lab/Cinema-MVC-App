using Core.DTOs;
using FluentValidation;

namespace Core.Validators;

public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.SessionId)
            .GreaterThan(0)
            .WithMessage("SessionId must be greater than 0");

        RuleFor(x => x.SeatIds)
            .NotNull()
            .NotEmpty()
            .WithMessage("You must select at least one seat");

        RuleForEach(x => x.SeatIds)
            .GreaterThan(0)
            .WithMessage("SeatId must be greater than 0");
    }
}
