using System.ComponentModel.DataAnnotations;

namespace TvShowTracker.Application.DTOs
{
    /// <summary>
    /// Data transfer object for creating new TV show records in the system.
    /// Contains comprehensive information needed to establish a complete TV show entry with metadata.
    /// </summary>
    public class CreateTvShowDto
    {
        /// <summary>
        /// The official title of the TV show as it appears in promotional materials and broadcasts.
        /// Should match the commonly recognized name used across media platforms.
        /// </summary>
        [Required(ErrorMessage = "Show name is required")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Show name must be between 1 and 200 characters")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// A comprehensive description or synopsis of the TV show's premise, plot, and themes.
        /// Should provide enough context for users to understand the show's content and appeal.
        /// </summary>
        [Required(ErrorMessage = "Description is required")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 2000 characters")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// The original premiere or first broadcast date of the TV show.
        /// Used for chronological organization and historical reference.
        /// </summary>
        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The current broadcast status of the TV show (e.g., "Running", "Ended", "Cancelled", "Hiatus").
        /// Indicates the show's production and broadcast state for user expectations.
        /// </summary>
        [Required(ErrorMessage = "Status is required")]
        [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// The primary broadcaster or streaming platform that originally airs or distributes the show.
        /// Identifies the content's source and may indicate content style or target audience.
        /// </summary>
        [Required(ErrorMessage = "Network is required")]
        [StringLength(100, ErrorMessage = "Network name cannot exceed 100 characters")]
        public string Network { get; set; } = string.Empty;

        /// <summary>
        /// URL pointing to the official poster or promotional image for the TV show.
        /// Used for visual identification and enhanced user interface presentation.
        /// </summary>
        [Url(ErrorMessage = "Invalid URL format for image")]
        [StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// Overall quality rating for the TV show on a scale of 0.0 to 10.0.
        /// Represents aggregated critical or audience assessment of the show's quality.
        /// </summary>
        [Range(0.0, 10.0, ErrorMessage = "Rating must be between 0.0 and 10.0")]
        public double Rating { get; set; }

        /// <summary>
        /// Collection of genre classifications that describe the show's content style and themes.
        /// Enables categorization, filtering, and recommendation functionality.
        /// </summary>
        [Required(ErrorMessage = "At least one genre is required")]
        [MinLength(1, ErrorMessage = "At least one genre must be specified")]
        public List<string> Genres { get; set; } = new();

        /// <summary>
        /// Classification of the show's format and production structure (e.g., "Series", "Miniseries", "Documentary").
        /// Indicates the show's intended format and episode structure expectations.
        /// </summary>
        [Required(ErrorMessage = "Show type is required")]
        [StringLength(50, ErrorMessage = "Show type cannot exceed 50 characters")]
        public string ShowType { get; set; } = string.Empty;
    }
}