using MovieLibrary.DTOs;
using MovieLibrary.Entities;

namespace MovieLibrary.Repositories
{
    public interface IActorsRepository
    {
        Task<int> Create(Actor actor);
        Task Delete(int id);
        Task<bool> Exist(int id);
        Task<List<int>> Exists(List<int> ids);
        Task<List<Actor>> GetAll(PaginationDTO pagination);
        Task<Actor?> GetById(int id);
        Task<List<Actor>> GetByName(string name, PaginationDTO pagination);
        Task Update(Actor actor);
    }
}