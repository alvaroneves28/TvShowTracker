using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvShowTracker.Application.DTOs;
using TvShowTracker.Core.Entities;

namespace TvShowTracker.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMaps();
        }

        private void CreateMaps()
        {
            // TvShow Mappings
            CreateMap<TvShow, TvShowDto>()
                .ForMember(dest => dest.EpisodeCount,
                          opt => opt.MapFrom(src => src.Episodes != null ? src.Episodes.Count : 0));

            CreateMap<TvShow, TvShowDetailDto>()
                .ForMember(dest => dest.EpisodeCount,
                          opt => opt.MapFrom(src => src.Episodes != null ? src.Episodes.Count : 0))
                .ForMember(dest => dest.Episodes,
                          opt => opt.MapFrom(src => src.Episodes))
                .ForMember(dest => dest.Actors,
                          opt => opt.MapFrom(src => src.Actors))
                .ForMember(dest => dest.IsFavorite,
                          opt => opt.Ignore()); // Será definido manualmente no service

            CreateMap<CreateTvShowDto, TvShow>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Episodes, opt => opt.Ignore())
                .ForMember(dest => dest.Actors, opt => opt.Ignore())
                .ForMember(dest => dest.UserFavorites, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            // Para updates - mapear para entidade existente
            CreateMap<CreateTvShowDto, TvShow>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Episodes, opt => opt.Ignore())
                .ForMember(dest => dest.Actors, opt => opt.Ignore())
                .ForMember(dest => dest.UserFavorites, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            // Episode Mappings
            CreateMap<Episode, EpisodeDto>()
                .ForMember(dest => dest.TvShowName,
                          opt => opt.MapFrom(src => src.TvShow != null ? src.TvShow.Name : string.Empty));

            CreateMap<CreateEpisodeDto, Episode>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TvShow, opt => opt.Ignore());

            // User Mappings
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.FavoritesCount,
                          opt => opt.MapFrom(src => src.Favorites != null ? src.Favorites.Count : 0));

            CreateMap<RegisterDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()) // Será definido no service
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Favorites, opt => opt.Ignore());

            // Actor Mappings
            CreateMap<Actor, ActorDto>();

            CreateMap<CreateActorDto, Actor>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TvShows, opt => opt.Ignore());

            // UserFavorite Mappings (se necessário)
            CreateMap<UserFavorite, UserFavoriteDto>()
                .ForMember(dest => dest.TvShow, opt => opt.MapFrom(src => src.TvShow))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));
        }
    }
}
