using ItineRoo.WebAPI.Models;

namespace ItineRoo.WebAPI.Interfaces
{
    public interface IUserService
    {
        Task<User?> FindUserByEmailAsync(string email);
        Task<User?> FindUserByTokenAsync(string verificationToken);
        bool SetRefreshToken(RefreshToken newRefreshToken, User user);
        bool CreateUser(User user);
        int Save();
        string GetMyAuthName();
        bool UpdateUser(User user);
    }
}
