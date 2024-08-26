using FluentValidation;
using MovieLibrary.DTOs;
using MovieLibrary.Validation;

namespace MovieLibrary.Filters
{
    public class CreateCommentDTOValidator: AbstractValidator<CreateCommentDTO>
    {
        public CreateCommentDTOValidator() 
        {
            RuleFor(comment => comment.body).NotEmpty().WithMessage(ValidationUtilities.NonEmptyMessage);
        }
    }
}
