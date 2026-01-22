using Core.DTOs;
using Core.Enums;
using FluentValidation;

namespace Core.Validators
{
    public class MovieCreateValidator : AbstractValidator<MovieDTO>
    {
        public MovieCreateValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(200);

            RuleFor(x => x.Description)
                .MaximumLength(2000); 

            RuleFor(x => x.Director)
                .NotEmpty().WithMessage("Director is required")
                .MaximumLength(100);
            
            RuleFor(x => x.Actors)
                .NotNull().WithMessage("Actors is required")
                .NotEmpty()
                .MaximumLength(1000);

            RuleFor(x => x.ReleaseDate)
                .InclusiveBetween(new DateTime(1895, 1, 1), DateTime.Today.AddYears(5))
                .WithMessage("Release date must be between the birth of cinema (1895) and the near future.");

            RuleFor(x => x.DurationMinutes)
                .InclusiveBetween((short)1, (short)600)
                .WithMessage("Duration must be between 1 and 600 minutes");

            RuleFor(x => x.AgeRestriction)
                .InclusiveBetween((byte)0, (byte)18)
                .WithMessage("Age restriction should be between 0 and 18");

            RuleFor(x => x.Rating)
                .InclusiveBetween(0m, 10m)
                .WithMessage("Rating must be between 0 and 10");
            
            RuleFor(x => x.Genres)
                .NotEqual(GenreType.None)
                .WithMessage("Please select at least one genre.");
            
            RuleFor(x => x.PosterUrl)
                .NotNull().WithMessage("PosterUrl is required")
                .NotEmpty()
                .Must(IsUrl)
                .WithMessage("Poster URL must be valid if provided")
                .MaximumLength(500);
            
            RuleFor(x => x.TrailerUrl)
                .Must(uri => string.IsNullOrEmpty(uri) || IsUrl(uri))
                .WithMessage("Trailer URL must be valid if provided");
        }
        private static bool IsUrl(string? link)
        {
            if (string.IsNullOrWhiteSpace(link)) return false;
            return Uri.TryCreate(link, UriKind.Absolute, out var uriResult)
                   && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}