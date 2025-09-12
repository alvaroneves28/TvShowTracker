using System.ComponentModel.DataAnnotations;

namespace TvShowTracker.Application.DTOs
{
    /// <summary>
    /// Data transfer object for creating new actor records in the system.
    /// Contains the essential information needed to establish an actor-character relationship within a TV show.
    /// </summary>
    public class CreateActorDto
    {
        /// <summary>
        /// The real name of the actor or performer.
        /// Should contain the actor's professionally known name for accurate identification.
        /// </summary>
        [Required(ErrorMessage = "Actor name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Actor name must be between 2 and 100 characters")]
        [RegularExpression(@"^[a-zA-Z\s\-\.\']+$", ErrorMessage = "Actor name can only contain letters, spaces, hyphens, periods, and apostrophes")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The name of the character portrayed by the actor in the TV show.
        /// Should match the character name as it appears in the show's official materials.
        /// </summary>
        [Required(ErrorMessage = "Character name is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Character name must be between 1 and 100 characters")]
        public string Character { get; set; } = string.Empty;

        /// <summary>
        /// Optional URL pointing to a promotional or headshot image of the actor.
        /// Used for visual identification in cast displays and user interfaces.
        /// </summary>
        [Url(ErrorMessage = "Invalid URL format for image")]
        [StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
        public string ImageUrl { get; set; } = string.Empty;
    }
}