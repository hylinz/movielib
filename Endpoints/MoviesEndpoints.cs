using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MovieLibrary.DTOs;
using MovieLibrary.Entities;
using MovieLibrary.Repositories;
using MovieLibrary.Services;

namespace MovieLibrary.Endpoints
{
    public static class MoviesEndpoints
    {
        private readonly static string container = "movies";
        public static RouteGroupBuilder MapMovies(this RouteGroupBuilder group)
        {
            group.MapPost("/", Create).DisableAntiforgery();
            group.MapGet("/{id:int}", GetById).CacheOutput(c => c.Expire(TimeSpan.FromMinutes(1)).Tag("movies-get"));
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromMinutes(1)).Tag("movies-get"));
            group.MapPut("/{id:int}", Update).DisableAntiforgery();
            group.MapDelete("/{id:int}", Delete).DisableAntiforgery();
            group.MapPost("/{id:int}/assignGenres", AssignGenres);
            group.MapPost("/{id:int}/assignActors", AssignActors);



            return group;
        }

        static async Task<Created<MovieDTO>> Create([FromForm] CreateMovieDTO createMovieDTO, IFileStorage fileStorage, IOutputCacheStore outputCacheStore, IMapper mapper, IMoviesRepository repository)
        {
            var movie = mapper.Map<Movie>(createMovieDTO);

            if (createMovieDTO.Poster is not null)
            {
                var url = await fileStorage.Store(container, createMovieDTO.Poster);
                movie.Poster = url;
            }

            var id = await repository.Create(movie);
            await outputCacheStore.EvictByTagAsync("movies-get", default);
            var movieDTO = mapper.Map<MovieDTO>(movie);
            return TypedResults.Created($"movies/{id}", movieDTO);
            
        }

        static async Task<Ok<List<MovieDTO>>> GetAll(IMoviesRepository repository, IMapper mapper, int page = 1, int recordsPerPAge = 10)
        {
            var pagination = new PaginationDTO { Page = page, RecordsPerPage = recordsPerPAge };
            var movies = await repository.GetAll(pagination);
            var movieDTO = mapper.Map<List<MovieDTO>>(movies);
            return TypedResults.Ok(movieDTO);
        }


        static async Task<Results<Ok<MovieDTO>, NotFound>> GetById(int id, IMoviesRepository repository, IMapper mapper)
        {
            var movie = await repository.GetById(id);
            if (movie is null)
            {
                return TypedResults.NotFound();
            }
            var movieDTO = mapper.Map<MovieDTO>(movie);
            return TypedResults.Ok(movieDTO);
        }


        static async Task<Results<NoContent, NotFound>> Update(int id, [FromForm] CreateMovieDTO createMovieDTO, IMoviesRepository repository, IFileStorage fileStorage, IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            var movieDB = await repository.GetById(id);
            if (movieDB is null)
            {
                return TypedResults.NotFound();
            }

            var movieForUpdate = mapper.Map<Movie>(createMovieDTO);
            movieForUpdate.Id = id;
            movieForUpdate.Poster = movieDB.Poster;

            if (createMovieDTO.Poster is not null)
            {
                var url = await fileStorage.Edit(movieForUpdate.Poster, container, createMovieDTO.Poster);
                movieForUpdate.Poster = url;
            }

            await repository.Update(movieForUpdate);
            await outputCacheStore.EvictByTagAsync("movies-get", default);
            return TypedResults.NoContent();


        }

        static async Task<Results<NoContent, NotFound>> Delete(int id, IMoviesRepository repository, IOutputCacheStore outputCacheStore, IFileStorage fileStorage)
        {
            var movieDB = await repository.GetById(id);
            if (movieDB is null)
            {
                return TypedResults.NotFound();
            }

            await repository.Delete(id);
            await fileStorage.Delete(movieDB.Poster, container);
            await outputCacheStore.EvictByTagAsync("movies-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound, BadRequest<String>>> AssignGenres (int id, List<int> genreIds, IMoviesRepository moviesRepository, IGenresRepository genresRepository)
        {
            if (! await moviesRepository.Exist(id))
            {
                return TypedResults.NotFound();
            }

            var existingGenres = new List<int>();

            if (genreIds.Count != 0)
            {
                existingGenres = await genresRepository.Exists(genreIds);
            }

            if (genreIds.Count != existingGenres.Count)
            {
                var nonExistingGenres = genreIds.Except(existingGenres);
                var nonExistingGenresCSV = string.Join(",", nonExistingGenres);
                return TypedResults.BadRequest($"The genre of ID {nonExistingGenresCSV} does not exist");
            }
            await moviesRepository.Assign(id, genreIds);

            return TypedResults.NoContent();

            
        }

        static async Task<Results<NoContent, NotFound, BadRequest<String>>> AssignActors(int id, List<AssignActorMovieDTO> actorsDTO, IMoviesRepository moviesRepository, IActorsRepository actorsRepository, IMapper mapper)
        {
            if (!await moviesRepository.Exist(id))
            {
                return TypedResults.NotFound();
            }

            var existingActors = new List<int>();
            var actorsIds = actorsDTO.Select(actor => actor.ActorId).ToList();

            if (actorsDTO.Count != 0)
            {
                existingActors = await actorsRepository.Exists(actorsIds);
            }

            if (existingActors.Count != actorsDTO.Count)
            {
                var nonExistingActors = actorsIds.Except(existingActors);
                var nonExistingActorsCSV = string.Join(",", nonExistingActors);
                return TypedResults.BadRequest($"The actors with ID: {nonExistingActorsCSV} does not exist");
            }

            var actors = mapper.Map<List<ActorMovie>>(actorsDTO);
            await moviesRepository.Assign(id, actors);
            return TypedResults.NoContent();






        }
    }
}
