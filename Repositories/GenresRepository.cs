using MovieLibrary.Entities;
using Microsoft.EntityFrameworkCore;

namespace MovieLibrary.Repositories
{
    public class GenresRepository : IGenresRepository
    {
        private readonly ApplicationDbContext context;
        public GenresRepository(ApplicationDbContext context) 
        {
            this.context = context;
        }


        public async Task<int> Create(Genre genre)
        {
            context.Add(genre);
            await context.SaveChangesAsync();
            return genre.Id;
        }

        public async Task Delete(int id)
        {
            await context.Genres.Where(genre => genre.Id == id).ExecuteDeleteAsync();
        }

        public async Task<bool> Exists(int id)
        {
            return await context.Genres.AnyAsync(genre => genre.Id == id);
        }

        public async Task<List<Genre>> GetAll()
        {
            return await context.Genres.OrderBy(genre => genre.Name).ToListAsync();
        }

        public async Task<Genre?> GetById(int id)
        {
            return await context.Genres.FirstOrDefaultAsync(genre => genre.Id == id);
        }

        public async Task Update(Genre genre)
        {
            context.Genres.Update(genre);
            await context.SaveChangesAsync();
        }
    }
}
