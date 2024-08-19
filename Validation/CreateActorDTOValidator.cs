using FluentValidation;
using MovieLibrary.DTOs;
using MovieLibrary.Repositories;

namespace MovieLibrary.Validation
{
    public class CreateActorDTOValidator : AbstractValidator<CreateActorDTO>
    {
        public CreateActorDTOValidator(IActorsRepository repository, IHttpContextAccessor httpContext)

        {
        var TodaysDate = DateTime.Now;
        var EarliestDate = new DateTime(1900,1,1);


        RuleFor(property => property.Name)
                .NotEmpty()
                .WithMessage(ValidationUtilities.NonEmptyMessage)
                .MaximumLength(150).WithMessage(ValidationUtilities.FieldSizeExceededMessage)
                .Must(ValidationUtilities.isTitledCased).WithMessage(ValidationUtilities.NonTitledCaseMessage);

     
        RuleFor(property => property.DateOfBirth)
                .GreaterThanOrEqualTo(EarliestDate)
                .WithMessage(ValidationUtilities.EarliestDateMessage(EarliestDate))
                .LessThan(TodaysDate)
                .WithMessage(ValidationUtilities.FutureDateNotAllowedMessage);
        }


    }
}
