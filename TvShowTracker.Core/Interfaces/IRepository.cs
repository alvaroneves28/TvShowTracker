using System.Linq.Expressions;

namespace TvShowTracker.Core.Interfaces
{
    /// <summary>
    /// Generic repository interface providing standardized data access operations for all domain entities.
    /// Implements the Repository pattern to abstract data persistence concerns and provide a consistent API.
    /// </summary>
    /// <typeparam name="T">Domain entity type that this repository manages</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Retrieves all entities of type T from the data store.
        /// Provides complete entity collection for scenarios requiring full dataset access.
        /// </summary>
        /// <returns>Complete collection of all entities in the repository</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Retrieves a single entity by its unique identifier.
        /// Provides direct entity access for scenarios requiring specific entity operations.
        /// </summary>
        /// <param name="id">Unique identifier of the entity to retrieve</param>
        /// <returns>Entity with matching identifier, or null if not found</returns>
        Task<T?> GetByIdAsync(int id);

        /// <summary>
        /// Retrieves entities matching the specified predicate expression.
        /// Provides flexible querying capabilities with compile-time safety and database optimization.
        /// </summary>
        /// <param name="predicate">Expression defining the filtering criteria</param>
        /// <returns>Collection of entities matching the specified criteria</returns>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Adds a new entity to the repository and returns the created entity with generated values.
        /// Supports entity creation with automatic key generation and audit trail establishment.
        /// </summary>
        /// <param name="entity">Entity instance to add to the repository</param>
        /// <returns>Created entity with generated identifier and system-managed properties</returns>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// Adds multiple entities to the repository in a single operation.
        /// Provides efficient bulk insertion capabilities with transaction consistency.
        /// </summary>
        /// <param name="entities">Collection of entities to add to the repository</param>
        /// <returns>Collection of created entities with generated identifiers and system properties</returns>
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Updates an existing entity with modified property values.
        /// Supports entity modification with change tracking and optimistic concurrency.
        /// </summary>
        /// <param name="entity">Entity instance with updated property values</param>
        /// <returns>Task representing the asynchronous update operation</returns>
        Task UpdateAsync(T entity);

        /// <summary>
        /// Removes an entity from the repository using the entity instance.
        /// Supports entity deletion with referential integrity and cascade handling.
        /// </summary>
        /// <param name="entity">Entity instance to remove from the repository</param>
        /// <returns>Task representing the asynchronous deletion operation</returns>
        Task DeleteAsync(T entity);

        /// <summary>
        /// Removes an entity from the repository using its unique identifier.
        /// Provides efficient deletion without requiring full entity retrieval.
        /// </summary>
        /// <param name="id">Unique identifier of the entity to remove</param>
        /// <returns>Task representing the asynchronous deletion operation</returns>
        Task DeleteByIdAsync(int id);

        /// <summary>
        /// Retrieves the total count of entities in the repository.
        /// Provides efficient entity counting without data retrieval overhead.
        /// </summary>
        /// <returns>Total number of entities in the repository</returns>
        Task<int> CountAsync();

        /// <summary>
        /// Checks whether an entity with the specified identifier exists in the repository.
        /// Provides efficient existence validation without entity retrieval overhead.
        /// </summary>
        /// <param name="id">Unique identifier to check for existence</param>
        /// <returns>True if entity exists, false otherwise</returns>
        Task<bool> ExistsAsync(int id);
    }
}