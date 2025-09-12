using Microsoft.EntityFrameworkCore.Storage;
using TvShowTracker.Core.Interfaces;
using TvShowTracker.Infrastructure.Data;

namespace TvShowTracker.Infrastructure.Repositories
{
    /// <summary>
    /// Implements the Unit of Work pattern for coordinating multiple repositories and database transactions.
    /// </summary>
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly TvShowContext _context;
        private IDbContextTransaction? _transaction;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
        /// Creates repository instances for TV shows, episodes, users, and user favorites.
        /// </summary>
        /// <param name="context">The <see cref="TvShowContext"/> instance for database access.</param>
        public UnitOfWork(TvShowContext context)
        {
            _context = context;
            TvShows = new TvShowRepository(_context);
            Episodes = new EpisodeRepository(_context);
            Users = new UserRepository(_context);
            UserFavorites = new UserFavoriteRepository(_context);
        }

        /// <summary>
        /// Gets the TV show repository.
        /// </summary>
        public ITvShowRepository TvShows { get; }

        /// <summary>
        /// Gets the episode repository.
        /// </summary>
        public IEpisodeRepository Episodes { get; }

        /// <summary>
        /// Gets the user repository.
        /// </summary>
        public IUserRepository Users { get; }

        /// <summary>
        /// Gets the user favorite repository.
        /// </summary>
        public IUserFavoriteRepository UserFavorites { get; }

        /// <summary>
        /// Saves all changes made in the current unit of work to the database.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Begins a new database transaction.
        /// </summary>
        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        /// <summary>
        /// Commits the current transaction.
        /// </summary>
        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        /// <summary>
        /// Rolls back the current transaction.
        /// </summary>
        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        /// <summary>
        /// Disposes the unit of work, including the current transaction and database context.
        /// </summary>
        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
