using ItineRoo.WebAPI.Interfaces;
using System.Security.Cryptography;

namespace ItineRoo.WebAPI.Services
{
    public class TokenService : ITokenService
    {
        /// <summary>
        /// CreatePasswordHash - Create the password hash
        /// </summary>
        /// <param name="password"></param>
        /// <param name="passwordHash"></param>
        /// <param name="passwordSalt"></param>
        /// <returns></returns>
        public bool CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac
                    .ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                return true;
            }
        }

        /// <summary>
        /// CreateRandomToken - create the token for verification
        /// </summary>
        /// <returns></returns>
        public string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }

        /// <summary>
        /// VerifyPasswordHash - verify the entered password to the hashed password
        /// </summary>
        /// <param name="password">Entered password</param>
        /// <param name="passwordHash"></param>
        /// <param name="passwordSalt"></param>
        /// <returns></returns>
        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac
                    .ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}
