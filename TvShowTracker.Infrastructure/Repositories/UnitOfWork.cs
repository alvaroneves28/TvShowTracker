using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvShowTracker.Core.Interfaces;
using TvShowTracker.Infrastructure.Data;

namespace TvShowTracker.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TvShowContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(TvShowContext context)
        {
            _context = context;
            TvShows = new TvShowRepository(_context);
            Episodes = new EpisodeRepository(_context);
            Users = new UserRepository(_context);
            UserFavorites = new UserFavoriteRepository(_context);
        }

        public ITvShowRepository TvShows { get; }
        public IEpisodeRepository Episodes { get; }
        public IUserRepository Users { get; }
        public IUserFavoriteRepository UserFavorites { get; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
