using Core.DTOs;
using FluentValidation;

namespace Core.Validators
{
    public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
    {
        public CreateOrderRequestValidator()
        {
            
            RuleFor(x => x.SessionId)
                .GreaterThan(0).WithMessage("Invalid SessionId. It must be greater than 0.");

           
            RuleFor(x => x.SeatIds)
                .NotEmpty().WithMessage("You must select at least one seat.")
                .NotNull().WithMessage("Seat list cannot be null.");

            
            RuleForEach(x => x.SeatIds)
                .GreaterThan(0).WithMessage("Invalid SeatId found in selection.");
        }
    }
}