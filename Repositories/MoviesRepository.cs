using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MovieLibrary.DTOs;
using MovieLibrary.Entities;

namespace MovieLibrary.Repositories
{
    public class MoviesRepository(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context, IMapper mapper) : IMoviesRepository
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
                .Include(movie => movie.GenresMovies)
                    .ThenInclude(genre => genre.Genre)
                .Include(movie => movie.ActorsMovies.OrderBy(am => am.Order))
                    .ThenInclude(actor => actor.Actor)
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

        public async Task Assign(int id, List<int> genresIds)
        {
            var movie = await context.Movies.Include(movie => movie.GenresMovies).FirstOrDefaultAsync(movie => movie.Id == id);

            if (movie is null)
            {
                throw new ArgumentException($"No movie found with id: {id}");
            }

            var genresMovies = genresIds.Select(genresId => new GenreMovie { GenreId = genresId });

            movie.GenresMovies = mapper.Map(genresMovies, movie.GenresMovies);

            await context.SaveChangesAsync();

        }

        public async Task Assign(int id, List<ActorMovie> actors)
        {
            for (int i = 1; i <= actors.Count; i++)
            {
                actors[i - 1].Order = i;
            }

            var movie = await context.Movies.Include(movie => movie.ActorsMovies)
                .FirstOrDefaultAsync(movie => movie.Id == id);

            if (movie is null)
            {
                throw new ArgumentException($"There is no movie with id {id}");
            }

            movie.ActorsMovies = mapper.Map(actors, movie.ActorsMovies);

            await context.SaveChangesAsync();

        }
    }
}
