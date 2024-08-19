using FluentValidation;
using MovieLibrary.DTOs;
using MovieLibrary.Repositories;

namespace MovieLibrary.Validation
{
    public class CreateGenreDTOValidator: AbstractValidator<CreateGenreDTO>
    {
        public CreateGenreDTOValidator(IGenresRepository genresRepository, IHttpContextAccessor httpContext) 
        {
            var routeId = httpContext.HttpContext!.Request.RouteValues["id"];
            var id = 0;

            if (routeId is string routeIdString)
            {
                int.TryParse(routeIdString, out id);
            }

            RuleFor(property => property.Name)
                .NotEmpty()
                .WithMessage(ValidationUtilities.NonEmptyMessage)
                .MaximumLength(150).WithMessage(ValidationUtilities.FieldSizeExceededMessage)
                .Must(ValidationUtilities.isTitledCased).WithMessage(ValidationUtilities.NonTitledCaseMessage)
                .MustAsync(async (name, _) =>
                {
                    var exists = await genresRepository.Exists(id, name);
                    return !exists;
                }).WithMessage(g => $"A genre with the name {g.Name} already exists");
        }
    }
}
