using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvShowTracker.Application.DTOs
{
    public class TvShowDetailDto : TvShowDto
    {
        public List<EpisodeDto> Episodes { get; set; } = new();
        public List<ActorDto> Actors { get; set; } = new();
        public bool IsFavorite { get; set; }
    }
}
