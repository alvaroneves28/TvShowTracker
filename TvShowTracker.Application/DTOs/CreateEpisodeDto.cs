using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvShowTracker.Application.DTOs
{
    public class CreateEpisodeDto
    {
        public string Name { get; set; } = string.Empty;
        public int Season { get; set; }
        public int EpisodeNumber { get; set; }
        public DateTime AirDate { get; set; }
        public string Summary { get; set; } = string.Empty;
        public double Rating { get; set; }
        public int TvShowId { get; set; }
    }
}
