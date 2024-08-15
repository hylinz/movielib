using Microsoft.EntityFrameworkCore;
using MovieLibrary.Entities;

namespace MovieLibrary.Repositories
{
    public class ActorsRepository(ApplicationDbContext context) : IActorsRepository
    {
        public async Task<List<Actor>> GetAll()
        {
            return await context.Actors.OrderBy(actor => actor.Name).ToListAsync();
        }
        public async Task<Actor?> GetById(int id)
        {
            return await context.Actors.AsNoTracking().FirstOrDefaultAsync(actor => actor.Id == id);
        }

        public async Task<List<Actor>> GetByName(string name)
        {
            return await context.Actors
                .Where(actor => actor.Name.Contains(name))
                .OrderBy(actor => actor.Name).ToListAsync();
        }

        public async Task<int> Create(Actor actor)
        {
            context.Add(actor);
            await context.SaveChangesAsync();
            return actor.Id;
        }

        public async Task<bool> Exist(int id)
        {
            return await context.Actors.AnyAsync(actor => actor.Id == id);
        }
        public async Task Update(Actor actor)
        {
            context.Update(actor);
            await context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            await context.Actors.Where(actor => actor.Id == id).ExecuteDeleteAsync();
        }
    }

}
