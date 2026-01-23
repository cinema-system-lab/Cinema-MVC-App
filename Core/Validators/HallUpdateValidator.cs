using Core.DTOs;
using Core.Enums;
using FluentValidation;

namespace Core.Validators;

public class HallUpdateValidator : AbstractValidator<HallDTO>
{
    public HallUpdateValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(50).WithMessage("Name cannot exceed 50 characters");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Hall type is invalid")
            .NotEqual((HallType)0).WithMessage("Hall type is required");
    }
}
