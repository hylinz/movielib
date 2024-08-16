using AutoMapper;
using MovieLibrary.DTOs;
using MovieLibrary.Entities;

namespace MovieLibrary.Utilities
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles() 
        {
            CreateMap<Genre, GenreDTO>();
            CreateMap<CreateGenreDTO, Genre>();
            CreateMap<Actor, ActorDTO>();
            CreateMap<CreateActorDTO, Actor>()
                .ForMember(property => property.Picture, options => options.Ignore());
            CreateMap<Movie, MovieDTO>();
            CreateMap<CreateMovieDTO, Movie>()
                .ForMember(property => property.Poster, options => options.Ignore());

        }
    }
}
