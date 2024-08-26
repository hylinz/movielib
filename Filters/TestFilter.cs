
using AutoMapper;
using Microsoft.AspNetCore.Components.Forms;
using MovieLibrary.Repositories;

namespace MovieLibrary.Filters
{
    public class TestFilter : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {

            var param1 = context.Arguments.OfType<int>().FirstOrDefault();
            var param2 = context.Arguments.OfType<IGenresRepository>().FirstOrDefault();
            var param3 = context.Arguments.OfType<IMapper>().FirstOrDefault();

            var result = await next(context);
            return result;
        }
    }
}
