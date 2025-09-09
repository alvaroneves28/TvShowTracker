using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvShowTracker.Core.Entities
{
    public class UserFavorite
    {
        public int UserId { get; set; }
        public int TvShowId { get; set; }
        public DateTime AddedAt { get; set; }
        public User User { get; set; }
        public TvShow TvShow { get; set; }
    }
}
