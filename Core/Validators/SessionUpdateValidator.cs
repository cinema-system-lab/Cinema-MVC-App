using Core.DTOs;
using FluentValidation;

namespace Core.Validators;

public class SessionUpdateValidator : AbstractValidator<SessionDTO>
{
    public SessionUpdateValidator()
    {
        RuleFor(x => x.MovieId)
            .GreaterThan(0);

        RuleFor(x => x.HallId)
            .GreaterThan(0);

        RuleFor(x => x.StartTime)
            .GreaterThan(DateTime.Now)
            .WithMessage("Session must start in the future");

        RuleFor(x => x.EndTime)
            .GreaterThan(x => x.StartTime)
            .WithMessage("EndTime must be after StartTime");

        RuleFor(x => x.BasePrice)
            .GreaterThan(0)
            .LessThan(10000);
    }
}