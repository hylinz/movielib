﻿using System.ComponentModel.DataAnnotations;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MovieLibrary.DTOs;
using MovieLibrary.Entities;
using MovieLibrary.Migrations;
using MovieLibrary.Repositories;

namespace MovieLibrary.Endpoints
{
    public static class GenresEndpoints
    {

       // ------------- Call Method
        public static RouteGroupBuilder MapGenres(this RouteGroupBuilder group)
        {
        group.MapGet("/", GetGenres).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(15)).Tag("genres-get"));
        group.MapGet("/{id:int}", GetById);
        group.MapPost("/", CreateGenre);
        group.MapPut("/{id:int}", UpdateGenre);
        group.MapDelete("/{id:int}", DeleteGenre);

            return group;
        }

        // ------------- Logic
        static async Task<Ok<List<GenreDTO>>> GetGenres(IGenresRepository repository, IMapper mapper)
        {
            var genres = await repository.GetAll();

            var genreDTO = mapper.Map<List<GenreDTO>>(genres);

            return TypedResults.Ok(genreDTO);
        }

        static async Task<Results<Ok<GenreDTO>, NotFound>> GetById(int id, IGenresRepository repository, IMapper mapper)
        {
            var genre = await repository.GetById(id);

            if (genre is null)
            {
                return TypedResults.NotFound();
            }

            var genreDTO = mapper.Map<GenreDTO>(genre);
            return TypedResults.Ok(genreDTO);
        }

        static async Task<Results<Created<GenreDTO>, ValidationProblem>> CreateGenre
            (CreateGenreDTO createGenreDTO, IGenresRepository repository, IOutputCacheStore outputCacheStore, IMapper mapper, IValidator<CreateGenreDTO> validator)
        {
            var validationResult = await validator.ValidateAsync(createGenreDTO);
            if (!validationResult.IsValid)
            {
                return TypedResults.ValidationProblem(validationResult.ToDictionary());

            }
            var genre = mapper.Map<Genre>(createGenreDTO);

            var id = await repository.Create(genre);
            // clean cache
            await outputCacheStore.EvictByTagAsync("genres-get", default);

            var genreDTO = mapper.Map<GenreDTO>(genre);

            return TypedResults.Created($"/genres/{id}", genreDTO);
        }


        static async Task<Results<NoContent, NotFound, ValidationProblem>> UpdateGenre
            (int id, CreateGenreDTO createGenreDTO, IGenresRepository repository, IOutputCacheStore outputCacheStore, IMapper mapper, IValidator<CreateGenreDTO> validator)
        {

            var validationResult = await validator.ValidateAsync(createGenreDTO);
            if (!validationResult.IsValid)
            {
                return TypedResults.ValidationProblem(validationResult.ToDictionary());

            }

            var exists = await repository.Exists(id);
            if (!exists)
            {
                return TypedResults.NotFound();
            }
            var genre = mapper.Map<Genre>(createGenreDTO);
            genre.Id = id;

            await repository.Update(genre);
            await outputCacheStore.EvictByTagAsync("genres-get", default);
            return TypedResults.NoContent();


        }

        static async Task<Results<NotFound, NoContent>> DeleteGenre(int id, IGenresRepository repository, IOutputCacheStore outputCacheStore)
        {
            var exists = await repository.Exists(id);
            if (!exists)
            {
                return TypedResults.NotFound();
            }

            await repository.Delete(id);
            await outputCacheStore.EvictByTagAsync("genres-get", default);
            return TypedResults.NoContent();


        }
    }
}
