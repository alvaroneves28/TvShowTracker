using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvShowTracker.Core.Entities
{
    public class Episode
    {
        public int Id { get; set; }
        public int TvShowId { get; set; }
        public string Name { get; set; }
        public int Season { get; set; }
        public int EpisodeNumber { get; set; }
        public DateTime AirDate { get; set; }
        public string Summary { get; set; }
        public double Rating { get; set; }
        public TvShow TvShow { get; set; }
    }
}
