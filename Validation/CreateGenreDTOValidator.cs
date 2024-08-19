using FluentValidation;
using MovieLibrary.DTOs;

namespace MovieLibrary.Validation
{
    public class CreateGenreDTOValidator: AbstractValidator<CreateGenreDTO>
    {
        public CreateGenreDTOValidator() 
        {
            RuleFor(property => property.Name).NotEmpty();
        }
    }
}
