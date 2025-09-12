namespace TvShowTracker.Application.DTOs
{
    /// <summary>
    /// Extended data transfer object representing a complete TV show with all related entities and user-specific data.
    /// Inherits from TvShowDto and adds comprehensive collections for episodes, cast, and personalized information.
    /// </summary>
    public class TvShowDetailDto : TvShowDto
    {
        /// <summary>
        /// Complete collection of episodes associated with this TV show.
        /// Organized chronologically by season and episode number for proper viewing order.
        /// </summary>
        public List<EpisodeDto> Episodes { get; set; } = new();

        /// <summary>
        /// Complete cast list including all actors and their character portrayals for this TV show.
        /// Provides comprehensive talent information for the production.
        /// </summary>
        public List<ActorDto> Actors { get; set; } = new();

        /// <summary>
        /// Indicates whether the authenticated user has marked this show as a favorite.
        /// Provides personalized context for user interface customization and behavior.
        /// </summary>
        public bool IsFavorite { get; set; }
    }
}