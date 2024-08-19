using AutoMapper;
using MovieLibrary.DTOs;
using MovieLibrary.Entities;

namespace MovieLibrary.Utilities
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles() 
        {
            // Genres
            CreateMap<Genre, GenreDTO>();
            CreateMap<CreateGenreDTO, Genre>();
            // Actors
            CreateMap<Actor, ActorDTO>();
            CreateMap<CreateActorDTO, Actor>()
                .ForMember(property => property.Picture, options => options.Ignore());
            CreateMap<AssignActorMovieDTO, ActorMovie>();
            // Movies
            CreateMap<Movie, MovieDTO>()
                .ForMember(x => x.Genres, entity => entity
                    .MapFrom(property => property.GenresMovies
                        .Select(gm => new GenreDTO { Id = gm.GenreId, Name = gm.Genre.Name })))
                .ForMember(x => x.Actors, entity => entity
                    .MapFrom(property => property.ActorsMovies.Select(am => new ActorMovieDTO
                    {
                        Id = am.ActorId,
                        Name = am.Actor.Name,
                        Character = am.Character
                    })));

            CreateMap<CreateMovieDTO, Movie>()
                .ForMember(property => property.Poster, options => options.Ignore());
            // Comments
            CreateMap<Comment, CommentDTO>();
            CreateMap<CreateCommentDTO, Comment>();


        }
    }
}
