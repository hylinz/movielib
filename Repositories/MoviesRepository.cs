using Microsoft.EntityFrameworkCore;
using MovieLibrary.DTOs;
using MovieLibrary.Entities;

namespace MovieLibrary.Repositories
{
    public class MoviesRepository(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context) : IMoviesRepository
    {
        public async Task<List<Movie>> GetAll(PaginationDTO pagination)
        {
            var queryable = context.Movies.AsQueryable();
            await httpContextAccessor.HttpContext!.InsertPaginationParameterInResponseHeader(queryable);
            return await queryable.OrderBy(movie => movie.Title).Paginate(pagination).ToListAsync();
        }
        public async Task<Movie?> GetById(int id)
        {
            return await context.Movies.AsNoTracking()
                .Include(movie => movie.Comments)
                .FirstOrDefaultAsync(movie => movie.Id == id);
        }

        public async Task<bool> Exist(int id)
        {
            return await context.Movies.AnyAsync(movie => movie.Id == id);
        }
        public async Task<int> Create(Movie movie)
        {
            context.Add(movie);
            await context.SaveChangesAsync();
            return movie.Id;
        }
        public async Task Update(Movie movie)
        {
            context.Update(movie);
            await context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            await context.Movies.Where(movie => movie.Id == id).ExecuteDeleteAsync();
        }
    }
}
