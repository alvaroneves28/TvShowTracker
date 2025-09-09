using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace TvShowTracker.Application.Interfaces
{
    public interface IAuthService
    {
        string GenerateJwtToken(int userId, string username, string email);
        ClaimsPrincipal? ValidateToken(string token);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
    }
}
