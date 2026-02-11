using Core.DTOs;
using Core.Enums;
using FluentValidation;

namespace Core.Validators
{
    public class MovieValidator : AbstractValidator<MovieDTO>
    {
        public MovieValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters");

            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .WithMessage("Description cannot exceed 1000 characters");

            RuleFor(x => x.Director)
                .NotEmpty().WithMessage("Director is required")
                .MaximumLength(100).WithMessage("Director cannot exceed 100 characters");
            
            RuleFor(x => x.Actors)
                .NotEmpty().WithMessage("Actors is required")
                .MaximumLength(1000).WithMessage("Actors cannot exceed 1000 characters");

            RuleFor(x => x.ReleaseDate)
                .Must(date => date >= new DateTime(1895, 1, 1) && date <= new DateTime(DateTime.Today.Year + 1, 12, 31))
                .WithMessage($"Release date must be between 1895 and the end of {DateTime.Today.Year + 1}");
            
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
                .WithMessage("Please select at least one genre");
            
            RuleFor(x => x.PosterUrl)
                .NotEmpty().WithMessage("Poster URL is required")
                .Must(IsUrl).WithMessage("Poster URL must be valid")
                .MaximumLength(300).WithMessage("Poster URL cannot exceed 300 characters");
            
            RuleFor(x => x.TrailerUrl)
                .Must(uri => string.IsNullOrEmpty(uri) || IsUrl(uri))
                .WithMessage("Trailer URL must be valid if provided")
                .MaximumLength(300).WithMessage("Trailer URL cannot exceed 300 characters");
        }

        private static bool IsUrl(string? link)
        {
            if (string.IsNullOrWhiteSpace(link)) return false;
            return Uri.TryCreate(link, UriKind.Absolute, out var uriResult)
                   && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}