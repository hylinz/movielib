using MovieLibrary.DTOs;
using MovieLibrary.Entities;

namespace MovieLibrary.Repositories
{
    public interface IMoviesRepository
    {
        Task Assign(int id, List<int> genresIds);
        Task<int> Create(Movie movie);
        Task Delete(int id);
        Task<bool> Exist(int id);
        Task<List<Movie>> GetAll(PaginationDTO pagination);
        Task<Movie?> GetById(int id);
        Task Update(Movie movie);
    }
}