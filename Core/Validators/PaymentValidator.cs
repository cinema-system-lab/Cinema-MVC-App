using Core.DTOs;
using Core.Enums;
using FluentValidation;

namespace Core.Validators;

public class PaymentValidator : AbstractValidator<PaymentDTO>
{
    public PaymentValidator()
    {
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Payment must be greater than 0");
        RuleFor(x => x.PaymentDate).LessThanOrEqualTo(DateTime.Now);
        RuleFor(x => x.OrderId).NotEmpty().WithMessage("Payment must be linked to an order");
    }
}