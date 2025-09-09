using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvShowTracker.Application.DTOs
{
    public class UserFavoriteDto
    {
        public int UserId { get; set; }
        public int TvShowId { get; set; }
        public DateTime AddedAt { get; set; }
        public UserDto? User { get; set; }
        public TvShowDto? TvShow { get; set; }
    }
}
