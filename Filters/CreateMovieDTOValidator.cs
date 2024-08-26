using FluentValidation;
using MovieLibrary.DTOs;
using MovieLibrary.Validation;

namespace MovieLibrary.Filters
{
    public class CreateMovieDTOValidator: AbstractValidator<CreateMovieDTO>
    {
        public CreateMovieDTOValidator() 
        {
            RuleFor(movie => movie.Title).NotEmpty().WithMessage(ValidationUtilities.NonEmptyMessage).MaximumLength(250).WithMessage(ValidationUtilities.FieldSizeExceededMessage);
        }
    }
}
