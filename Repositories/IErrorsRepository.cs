using MovieLibrary.Entities;

namespace MovieLibrary.Repositories
{
    public interface IErrorsRepository
    {
        Task Create(Error error);
    }
}