using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvShowTracker.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ITvShowRepository TvShows { get; }
        IEpisodeRepository Episodes { get; }
        IUserRepository Users { get; }
        IUserFavoriteRepository UserFavorites { get; }
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
