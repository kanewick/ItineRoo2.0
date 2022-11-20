using ItineRoo.WebAPI.Models;

namespace ItineRoo.WebAPI.Interfaces
{
    public interface IAuthTokenService
    {
        string CreateToken(User user, IConfiguration _configuration);
        RefreshToken GenerateRefreshToken();
    }
}
