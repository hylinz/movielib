
using FluentValidation;
using MovieLibrary.DTOs;

namespace MovieLibrary.Filters
{
    public class GenresValidationFilter : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var validator = context.HttpContext.RequestServices.GetService<IValidator<CreateGenreDTO>>();

            if (validator is null) 
            {
                return await next(context);
            }

            var obj = context.Arguments.OfType<CreateGenreDTO>().FirstOrDefault();

            if (obj is null)
            {
                return Results.Problem("Could not find object to validate");
            }

            var validationResult = await validator.ValidateAsync(obj);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }
            return await next(context);
        }
    }
}
