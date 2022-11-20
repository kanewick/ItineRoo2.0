using ItineRoo.WebAPI.Data;
using ItineRoo.WebAPI.Interfaces;
using ItineRoo.WebAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ItineRoo.WebAPI.Services
{
    public class UserService : IUserService
    {
        private readonly ItineRooContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserService(
            ItineRooContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Save - Save Context Changes
        /// </summary>
        /// <returns></returns>
        public Task Save() => _context.SaveChangesAsync();

        /// <summary>
        /// FindUserByEmailAsync - Gets user by email
        /// </summary>
        /// <param name="email">User email</param>
        /// <returns></returns>
        public Task<User?> FindUserByEmailAsync(string email)
            => _context.Users.FirstOrDefaultAsync(x => x.Email == email);

        /// <summary>
        /// FindUserByTokenAsync - Find user by the verification token / confirm register token
        /// </summary>
        /// <param name="verificationToken">Confirmation of register token</param>
        /// <returns></returns>
        public Task<User?> FindUserByTokenAsync(string verificationToken)
            => _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == verificationToken);

        /// <summary>
        /// CreateUser - creates the user
        /// </summary>
        /// <param name="user">User Model</param>
        public bool CreateUser(User user)
        {
            try
            {
                // Add user to the db
                _context.Users.Add(user);

                // Save
                Save();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// UpdateUser - Updates the user
        /// </summary>
        /// <param name="user">User Model</param>
        public bool UpdateUser(User user)
        {
            try
            {
                // Grab the user by the id
                var existingUser = _context.Users.FirstOrDefault(x => x.Id == user.Id);

                // check if the user exists
                if (existingUser is null) return false;

                // Update the existsing db user
                _context.Entry(existingUser).CurrentValues.SetValues(user);

                Save();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// GetMyAuthName - Gets name from Identity set up with JWT
        /// </summary>
        /// <returns></returns>
        public string GetMyAuthName()
        {
            try
            {
                // Grab the username from the identity claims
                var result = string.Empty;
                if (_httpContextAccessor.HttpContext != null)
                {
                    result = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
                }

                return result;
            }
            catch (Exception e)
            {
                return "Name could not be found";
            }
        }

        /// <summary>
        /// SetRefreshToken - Creates the cookie based of the token (jwt) and adds to the context (session)
        /// </summary>
        /// <param name="newRefreshToken">Authentication Token</param>
        /// <param name="user">User</param>
        public bool SetRefreshToken(RefreshToken refreshToken, User user)
        {
            try
            {
                // Create the browser cookie
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = refreshToken.Expires
                };

                // If the httpcontext is still alive, then set the cookie
                if (_httpContextAccessor.HttpContext != null)
                {
                    _httpContextAccessor.HttpContext.Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
                }

                // Apply the session cookie details to the user
                user.AuthRefreshToken = refreshToken.Token;
                user.AuthTokenCreated = refreshToken.Created;
                user.AuthTokenExpires = refreshToken.Expires;

                // Update the user
                return UpdateUser(user);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
