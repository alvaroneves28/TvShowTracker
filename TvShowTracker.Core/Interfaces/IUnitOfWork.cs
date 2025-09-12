namespace TvShowTracker.Core.Interfaces
{
    /// <summary>
    /// Unit of Work interface coordinating multiple repository operations within transactional boundaries.
    /// Provides centralized transaction management and ensures data consistency across related entity operations.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Repository for TV show entity operations and specialized querying.
        /// Provides access to comprehensive show management and content discovery functionality.
        /// </summary>
        ITvShowRepository TvShows { get; }

        /// <summary>
        /// Repository for episode entity operations and hierarchical content organization.
        /// Provides access to episode management and show content structure functionality.
        /// </summary>
        IEpisodeRepository Episodes { get; }

        /// <summary>
        /// Repository for user entity operations and account management functionality.
        /// Provides access to user authentication, profile management, and security operations.
        /// </summary>
        IUserRepository Users { get; }

        /// <summary>
        /// Repository for user favorite relationship management and personalization functionality.
        /// Provides access to user preference tracking and personalized content experiences.
        /// </summary>
        IUserFavoriteRepository UserFavorites { get; }

        /// <summary>
        /// Persists all pending changes across all repositories within the current Unit of Work context.
        /// Ensures atomic persistence of all entity modifications within a single transaction.
        /// </summary>
        /// <returns>Number of entities affected by the save operation</returns>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// Initiates an explicit database transaction for complex operations requiring precise control.
        /// Enables manual transaction management for scenarios requiring custom transaction boundaries.
        /// </summary>
        /// <returns>Task representing the asynchronous transaction initiation</returns>
        Task BeginTransactionAsync();

        /// <summary>
        /// Commits the current explicit transaction, making all changes permanent.
        /// Finalizes all operations within the transaction boundary ensuring data persistence.
        /// </summary>
        /// <returns>Task representing the asynchronous transaction commit operation</returns>
        Task CommitTransactionAsync();

        /// <summary>
        /// Rolls back the current explicit transaction, discarding all changes.
        /// Reverts all operations within the transaction boundary ensuring data consistency.
        /// </summary>
        /// <returns>Task representing the asynchronous transaction rollback operation</returns>
        Task RollbackTransactionAsync();
    }
}