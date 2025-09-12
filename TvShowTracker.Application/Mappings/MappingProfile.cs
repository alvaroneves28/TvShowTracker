using AutoMapper;
using TvShowTracker.Application.DTOs;
using TvShowTracker.Core.Entities;

namespace TvShowTracker.Application.Mappings
{
    /// <summary>
    /// AutoMapper configuration profile defining object-to-object mapping rules for the TV Show Tracker application.
    /// Establishes comprehensive mapping relationships between domain entities and data transfer objects.
    /// </summary>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// Initializes the mapping profile and configures all entity-to-DTO mapping relationships.
        /// </summary>
        public MappingProfile()
        {
            CreateMaps();
        }

        /// <summary>
        /// Configures comprehensive mapping rules for all application entities and DTOs.
        /// Establishes bidirectional mappings with proper security and data integrity considerations.
        /// </summary>
        private void CreateMaps()
        {
            #region TvShow Mappings

            /// <summary>
            /// Maps TvShow entity to basic TvShowDto for general display and listing scenarios.
            /// Includes calculated episode count for user information without loading full episode collection.
            /// </summary>
            CreateMap<TvShow, TvShowDto>()
                .ForMember(dest => dest.EpisodeCount,
                          opt => opt.MapFrom(src => src.Episodes != null ? src.Episodes.Count : 0));

            /// <summary>
            /// Maps TvShow entity to detailed TvShowDetailDto for comprehensive show information display.
            /// Includes related entities (episodes, actors) and user context placeholder.
            /// </summary>
            CreateMap<TvShow, TvShowDetailDto>()
                .ForMember(dest => dest.EpisodeCount,
                          opt => opt.MapFrom(src => src.Episodes != null ? src.Episodes.Count : 0))
                .ForMember(dest => dest.Episodes,
                          opt => opt.MapFrom(src => src.Episodes))
                .ForMember(dest => dest.Actors,
                          opt => opt.MapFrom(src => src.Actors))
                .ForMember(dest => dest.IsFavorite,
                          opt => opt.Ignore()); // Populated manually in service layer with user context

            /// <summary>
            /// Maps CreateTvShowDto to TvShow entity for new show creation scenarios.
            /// Excludes system-managed properties and related entities for clean entity creation.
            /// </summary>
            CreateMap<CreateTvShowDto, TvShow>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())                    // Auto-generated
                .ForMember(dest => dest.Episodes, opt => opt.Ignore())             // Managed separately
                .ForMember(dest => dest.Actors, opt => opt.Ignore())               // Managed separately
                .ForMember(dest => dest.UserFavorites, opt => opt.Ignore())        // User-driven creation
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())            // System timestamp
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());           // System timestamp

            // Note: Duplicate mapping removed - CreateTvShowDto to TvShow mapping handles both create and update scenarios
            // Update operations use the same mapping with entity merging handled in the service layer

            #endregion

            #region Episode Mappings

            /// <summary>
            /// Maps Episode entity to EpisodeDto with show context for comprehensive episode information.
            /// Includes parent show name for context-rich episode displays without requiring separate lookups.
            /// </summary>
            CreateMap<Episode, EpisodeDto>()
                .ForMember(dest => dest.TvShowName,
                          opt => opt.MapFrom(src => src.TvShow != null ? src.TvShow.Name : string.Empty));

            /// <summary>
            /// Maps CreateEpisodeDto to Episode entity for new episode creation.
            /// Excludes system-managed properties while preserving core episode metadata.
            /// </summary>
            CreateMap<CreateEpisodeDto, Episode>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())                   // Auto-generated
                .ForMember(dest => dest.TvShow, opt => opt.Ignore());              // Managed by foreign key

            #endregion

            #region User Mappings

            /// <summary>
            /// Maps User entity to UserDto with engagement statistics for profile and display purposes.
            /// Includes calculated favorite count for immediate user engagement information.
            /// </summary>
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.FavoritesCount,
                          opt => opt.MapFrom(src => src.Favorites != null ? src.Favorites.Count : 0));

            /// <summary>
            /// Maps RegisterDto to User entity for new user account creation.
            /// Excludes security-sensitive and system-managed properties for safe entity creation.
            /// </summary>
            CreateMap<RegisterDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())                   // Auto-generated
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())         // Security - handled in service
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())            // System timestamp
                .ForMember(dest => dest.Favorites, opt => opt.Ignore());           // User-driven creation

            #endregion

            #region Actor Mappings

            /// <summary>
            /// Maps Actor entity to ActorDto for straightforward actor information display.
            /// Provides direct mapping for actor profile and character information.
            /// </summary>
            CreateMap<Actor, ActorDto>();

            /// <summary>
            /// Maps CreateActorDto to Actor entity for new actor record creation.
            /// Excludes system-managed properties while preserving actor and character information.
            /// </summary>
            CreateMap<CreateActorDto, Actor>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())                   // Auto-generated
                .ForMember(dest => dest.TvShows, opt => opt.Ignore());             // Managed separately

            #endregion

            #region UserFavorite Mappings

            /// <summary>
            /// Maps UserFavorite entity to UserFavoriteDto with optional related entity inclusion.
            /// Supports flexible data projection based on query requirements and performance needs.
            /// </summary>
            CreateMap<UserFavorite, UserFavoriteDto>()
                .ForMember(dest => dest.TvShow, opt => opt.MapFrom(src => src.TvShow))     // Optional inclusion
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));        // Optional inclusion

            #endregion
        }
    }
}