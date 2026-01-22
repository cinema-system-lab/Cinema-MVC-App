using Core.DTOs;
using FluentValidation;

namespace Core.Validators
{
    public class MovieUpdateValidator : AbstractValidator<MovieDTO>
    {
        public MovieUpdateValidator()
        {
            RuleFor(x => x.Title)
                .NotNull().WithMessage("Title is required.")
                .NotEmpty().WithMessage("Title cannot be empty.")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

            RuleFor(x => x.Rating)
                .InclusiveBetween(0m, 10m).WithMessage("Rating must be between 0 and 10.");
            
            RuleFor(x => x.DurationMinutes)
                .GreaterThan((short)0).WithMessage("Duration must be greater than zero.");

            RuleFor(x => x.PosterUrl)
                .MaximumLength(300).WithMessage("PosterUrl cannot exceed 300 characters.")
                .Must(url => string.IsNullOrEmpty(url) || IsUrl(url))
                .WithMessage("PosterUrl must be a valid URL if provided.");

            RuleFor(x => x.TrailerUrl)
                .MaximumLength(300).WithMessage("TrailerUrl cannot exceed 300 characters.")
                .Must(url => string.IsNullOrEmpty(url) || IsUrl(url))
                .WithMessage("TrailerUrl must be a valid URL if provided.");
        }

        private static bool IsUrl(string? link)
        {
            if (string.IsNullOrWhiteSpace(link))
                return false;

            return Uri.TryCreate(link, UriKind.Absolute, out var outUri)
                   && (outUri.Scheme == Uri.UriSchemeHttp || outUri.Scheme == Uri.UriSchemeHttps);
        }
    }
}