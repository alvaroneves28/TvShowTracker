using System.ComponentModel.DataAnnotations;

namespace TvShowTracker.Application.DTOs
{
    /// <summary>
    /// Data transfer object representing an actor and their character information within a TV show.
    /// Contains both the real actor's details and their fictional character portrayal.
    /// </summary>
    public class ActorDto
    {
        /// <summary>
        /// Unique identifier for the actor record in the system.
        /// Used for referencing and linking actor data across the application.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The real name of the actor or performer.
        /// Represents the actual person who portrays the character.
        /// </summary>
        [Required(ErrorMessage = "Actor name is required")]
        [StringLength(100, ErrorMessage = "Actor name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The name of the fictional character portrayed by the actor in the TV show.
        /// Represents the role or character identity within the show's narrative.
        /// </summary>
        [Required(ErrorMessage = "Character name is required")]
        [StringLength(100, ErrorMessage = "Character name cannot exceed 100 characters")]
        public string Character { get; set; } = string.Empty;

        /// <summary>
        /// URL pointing to a promotional or headshot image of the actor.
        /// Used for visual identification and display in user interfaces.
        /// </summary>
        [Url(ErrorMessage = "Invalid URL format for image")]
        [StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
        public string ImageUrl { get; set; } = string.Empty;
    }
}