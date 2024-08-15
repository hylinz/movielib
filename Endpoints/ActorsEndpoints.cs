using AutoMapper;
using MovieLibrary.Entities;
using Microsoft.AspNetCore.OutputCaching;
using MovieLibrary.Repositories;
using MovieLibrary.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MovieLibrary.Services;

namespace MovieLibrary.Endpoints
{
    public static class ActorsEndpoints
    {
        public static RouteGroupBuilder MapActors(this RouteGroupBuilder group)
        {
            group.MapPost("/", Create).DisableAntiforgery();
            return group;
        }

        static async Task<Created<ActorDTO>> Create([FromForm] CreateActorDTO createActorDTO, IActorsRepository repository, IOutputCacheStore outputCacheStore, IMapper mapper, IFileStorage fileStorage)
        {
            var actor = mapper.Map<Actor>(createActorDTO);

            if (createActorDTO.Picture is not null)
            {

            }

            var id = await repository.Create(actor);
            await outputCacheStore.EvictByTagAsync("actor-get", default);
            var actorDTO = mapper.Map<ActorDTO>(actor);
            return TypedResults.Created($"/actors/{id}", actorDTO);
        }
    }
}
