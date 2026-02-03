using Core.DTOs;
using FluentValidation;

namespace Core.Validators;

public class PaymentValidator : AbstractValidator<CreatePaymentDTO>
{
    public PaymentValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Payment amount must be greater than 0");
            
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Payment must be linked to an order");
    }
}