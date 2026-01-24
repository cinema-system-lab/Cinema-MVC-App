using Core.DTOs;
using FluentValidation;

namespace Core.Validators;

public class SeatGenerationValidator : AbstractValidator<SeatGenerationDTO>
{
    public SeatGenerationValidator()
    {
        RuleFor(x => x.HallId).GreaterThan(0);
        
        RuleFor(x => x.Rows)
            .InclusiveBetween(1, 50)
            .WithMessage("Rows must be between 1 and 50.");

        RuleFor(x => x.SeatsPerRow)
            .InclusiveBetween(1, 50)
            .WithMessage("Seats per row must be between 1 and 50.");
    }
}