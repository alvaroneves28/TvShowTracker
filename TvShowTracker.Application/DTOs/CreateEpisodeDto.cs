using System.ComponentModel.DataAnnotations;

namespace TvShowTracker.Application.DTOs
{
    /// <summary>
    /// Data transfer object for creating new episode records in the system.
    /// Contains all necessary information to establish a complete episode entry linked to a specific TV show.
    /// </summary>
    public class CreateEpisodeDto
    {
        /// <summary>
        /// The official title of the episode as it appears in show materials.
        /// Should match the episode title used in official broadcasts and documentation.
        /// </summary>
        [Required(ErrorMessage = "Episode name is required")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Episode name must be between 1 and 200 characters")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The season number this episode belongs to, starting from 1.
        /// Represents the organizational grouping of episodes within the TV show structure.
        /// </summary>
        [Range(1, 50, ErrorMessage = "Season must be between 1 and 50")]
        public int Season { get; set; }

        /// <summary>
        /// The episode number within its specific season, starting from 1.
        /// Determines the viewing order and position of the episode within the season.
        /// </summary>
        [Range(1, 500, ErrorMessage = "Episode number must be between 1 and 500")]
        public int EpisodeNumber { get; set; }

        /// <summary>
        /// The original broadcast date and time when the episode first aired.
        /// Used for chronological sorting and historical reference.
        /// </summary>
        [Required(ErrorMessage = "Air date is required")]
        public DateTime AirDate { get; set; }

        /// <summary>
        /// A brief, spoiler-free summary describing the episode's main plot or themes.
        /// Should provide enough context to help viewers decide whether to watch.
        /// </summary>
        [StringLength(1000, ErrorMessage = "Summary cannot exceed 1000 characters")]
        public string Summary { get; set; } = string.Empty;

        /// <summary>
        /// The critical or audience rating for the episode on a scale of 0.0 to 10.0.
        /// Represents the general quality assessment or viewer satisfaction.
        /// </summary>
        [Range(0.0, 10.0, ErrorMessage = "Rating must be between 0.0 and 10.0")]
        public double Rating { get; set; }

        /// <summary>
        /// The unique identifier of the TV show this episode belongs to.
        /// Establishes the parent-child relationship between show and episode.
        /// </summary>
        [Required(ErrorMessage = "TV Show ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "TV Show ID must be a positive number")]
        public int TvShowId { get; set; }
    }
}