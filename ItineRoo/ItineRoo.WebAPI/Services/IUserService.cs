using DataAccess.Models;

namespace DataAccess.Services
{
    public interface IUserService
    {
        UserModel? GetUser(int id);
    }
}