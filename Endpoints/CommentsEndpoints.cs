using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using MovieLibrary.Entities;
using Microsoft.AspNetCore.OutputCaching;
using MovieLibrary.DTOs;
using MovieLibrary.Repositories;
using MovieLibrary.Filters;

namespace MovieLibrary.Endpoints
{
    public static class CommentsEndpoints
    {
        public static RouteGroupBuilder MapComments(this RouteGroupBuilder group)
        {
            group.MapPost("/", Create).AddEndpointFilter<ValidationFilter<CreateCommentDTO>>();
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(15)).Tag("comments-get"));
            group.MapGet("/{id:int}", GetById).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(15)).Tag("comments-get"));
            group.MapPut("/{id:int}", Update).AddEndpointFilter<ValidationFilter<CreateCommentDTO>>();
            group.MapDelete("/{id:int}", Delete);


            return group;
        }

        static async Task<Results<Ok<List<CommentDTO>>, NotFound>> GetAll(int movieId, ICommentsRepository commentsRepository, IMoviesRepository moviesRepository, IMapper mapper)
        {
            if (!await moviesRepository.Exist(movieId))
            {
                return TypedResults.NotFound();
            }

            var comments = await commentsRepository.GetAll(movieId);
            var commentsDTO = mapper.Map<List<CommentDTO>>(comments);
            return TypedResults.Ok(commentsDTO);
        }

        static async Task<Results<Ok<CommentDTO>, NotFound>> GetById(int movieId, int id, ICommentsRepository commentsRepository, IMoviesRepository moviesRepository, IMapper mapper)
        {
            if (!await moviesRepository.Exist(movieId))
            {
                return TypedResults.NotFound();
            }

            var comment = await commentsRepository.GetById(id);
            if (comment is null)
            {
                return TypedResults.NotFound();
            }

            var commentsDTO = mapper.Map<CommentDTO>(comment);
            return TypedResults.Ok(commentsDTO);
        }

        static async Task<Results<Created<CommentDTO>, NotFound>> Create(int movieId, CreateCommentDTO createCommentDTO, ICommentsRepository commentsRepository, IMoviesRepository moviesRepository, IMapper mapper, IOutputCacheStore outputCacheStore)
        {
            if (! await moviesRepository.Exist(movieId)) 
            {
                return TypedResults.NotFound();
            }


            var comment = mapper.Map<Comment>(createCommentDTO);
            comment.MovieId = movieId;
            var id = await commentsRepository.Create(comment);
            await outputCacheStore.EvictByTagAsync("comments-get", default);
            var commentDTO = mapper.Map<CommentDTO>(comment);
            return TypedResults.Created($"/comment/{id}", commentDTO);
        }

        static async Task<Results<NoContent, NotFound>> Update(int movieId, int id, CreateCommentDTO createCommentDTO, IOutputCacheStore outputCacheStore, ICommentsRepository commentsRepository, IMapper mapper, IMoviesRepository moviesRepository)
        {
            if (!await moviesRepository.Exist(movieId))
            {
                return TypedResults.NotFound();
            }
            if (!await commentsRepository.Exists(id))
            {
                return TypedResults.NotFound();
            }

            var comment = mapper.Map<Comment>(createCommentDTO);
            comment.Id = id;
            comment.MovieId = movieId;
            await commentsRepository.Update(comment);
            await outputCacheStore.EvictByTagAsync("comments-get", default);
            return TypedResults.NoContent();
        }
        static async Task<Results<NoContent, NotFound>> Delete(int movieId, int id, IOutputCacheStore outputCacheStore, ICommentsRepository commentsRepository, IMoviesRepository moviesRepository)
        {
            if (!await moviesRepository.Exist(movieId))
            {
                return TypedResults.NotFound();
            }
            if (!await commentsRepository.Exists(id))
            {
                return TypedResults.NotFound();
            }

            await commentsRepository.Delete(id);
            await outputCacheStore.EvictByTagAsync("comments-get", default);


            return TypedResults.NoContent();
        }


    }
}
