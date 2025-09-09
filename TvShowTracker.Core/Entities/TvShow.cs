using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TvShowTracker.Core.Entities
{
    public class TvShow
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public string Status { get; set; } // Running, Ended, etc.
        public string Network { get; set; }
        public string ImageUrl { get; set; }
        public double Rating { get; set; }
        public List<string> Genres { get; set; }
        public string ShowType { get; set; } // Series, Miniseries, etc.
        public List<Episode> Episodes { get; set; }
        public List<Actor> Actors { get; set; }
        public List<UserFavorite> UserFavorites { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
