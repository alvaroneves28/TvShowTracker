namespace TvShowTracker.Core.Entities
{
    /// <summary>
    /// Domain entity representing an actor and their character portrayal within TV show productions.
    /// Encapsulates the relationship between real-world performers and their fictional character roles.
    /// </summary>
    public class Actor
    {
        /// <summary>
        /// Unique identifier for the actor entity within the system.
        /// Serves as the primary key for database operations and relationship establishment.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Professional name of the actor as recognized in the entertainment industry.
        /// Represents the performer's commonly known identity for public recognition.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Name of the fictional character portrayed by the actor within specific TV show contexts.
        /// Establishes the narrative role and character identity within show storylines.
        /// </summary>
        public string Character { get; set; }

        /// <summary>
        /// URL pointing to professional headshot or promotional image of the actor.
        /// Provides visual identification and enhances user interface presentation.
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Navigation property representing the many-to-many relationship with TV shows.
        /// Establishes the collection of productions in which this actor has appeared.
        /// </summary>
        public List<TvShow> TvShows { get; set; }
    }
}