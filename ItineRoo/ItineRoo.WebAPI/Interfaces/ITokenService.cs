namespace ItineRoo.WebAPI.Interfaces
{
    public interface ITokenService
    {
        bool CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
        string CreateRandomToken();
    }
}
