using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvShowTracker.Core.Entities
{
    public class Actor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Character { get; set; }
        public string ImageUrl { get; set; }
        public List<TvShow> TvShows { get; set; }
    }
}
