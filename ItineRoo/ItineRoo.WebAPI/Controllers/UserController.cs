﻿using ItineRoo.Dto;
using ItineRoo.WebAPI.Data;
using ItineRoo.WebAPI.Interfaces;
using ItineRoo.WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace ItineRoo.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly IAuthTokenService _authTokenService;

        public UserController(
            IUserService userService,
            ITokenService tokenService,
            IConfiguration configuration,
            IAuthTokenService authTokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
            _configuration = configuration;
            _authTokenService = authTokenService;
        }

        /// <summary>
        /// GetMe - Gets name from ClaimsPrincipal utilising session cookie based of JWT
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public IActionResult GetMe()
        {
            try
            {
                // Grab username from claims principal using httpcontext
                var userName = _userService.GetMyAuthName();
                return Ok(userName);
            }
            catch (Exception)
            {
                return BadRequest("Sorry, a problem occured.");
            }
        }

        /// <summary>
        /// Register - used for registering user / creates user and verification token
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterRequestModel request)
        {
            try
            {
                // Validate user doesnt already exist
                if (await _userService.FindUserByEmailAsync(request.Email) != null)
                {
                    return WebApiResponse.Error<string>("User already exists.");
                }

                // Create the hashed password and salt
                if (!_tokenService.CreatePasswordHash(request.Password,
                     out byte[] passwordHash,
                     out byte[] passwordSalt)) return WebApiResponse.Error<string>("Password creation error.");

                // Create the user with the verification token that needs to be verified
                var user = new User
                {
                    Email = request.Email,
                    Username = request.Username,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    VerificationToken = _tokenService.CreateRandomToken()
                };

                // Create the user
                if (!_userService.CreateUser(user)) return WebApiResponse.Error<string>("User could not be created");

                return WebApiResponse.Success(request);
            }
            catch (Exception)
            {
                return WebApiResponse.Error<string>("Exception occured");
            }
        }

        /// <summary>
        /// Login - used for logging in and creating session cookie with JWT
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginRequest request)
        {
            try
            {
                // Get user by email
                var user = await _userService.FindUserByEmailAsync(request.Email);

                if (user == null)
                {
                    return WebApiResponse.Error<string>("User not found.");
                }

                // Hash and salt the requested password and compare to users db password
                if (!_tokenService.VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
                {
                    return WebApiResponse.Error<string>("Cannot verify Password hash");
                }

                // Check if user has been verified
                if (user.VerifiedAt == null)
                {
                    return WebApiResponse.Error<string>("User not verified");
                }

                return WebApiResponse.Success(CreateTokensAndGetJWT(user));
            }
            catch (Exception)
            {
                return BadRequest("Sorry, a problem occured.");
            }
        }

        /// <summary>
        /// Verify - Used for verifying the registered user / confirm email for example.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost("verify")]
        public async Task<IActionResult> Verify([FromBody]string token)
        {
            try
            {
                // Grab the user by the verification token
                var user = await _userService.FindUserByTokenAsync(token);

                // If user not found, token is invalid
                if (user == null)
                {
                    return WebApiResponse.Error<string>("Invalid Token.");
                }

                // Set the verified date to the time they logged in
                user.VerifiedAt = DateTime.Now;

                // Update the user
                if (!_userService.UpdateUser(user)) return WebApiResponse.Error<string>("User could not be updated.");

                return WebApiResponse.Success(new UserVerifyResponseModel
                {
                    Token = token
                });

            }
            catch (Exception)
            {
                return WebApiResponse.Error<string>("Sorry, a problem occured.");
            }
        }

        private string CreateTokensAndGetJWT(User user)
        {

            // Users logged in successfully now create jwt for authenticated requests
            // At this point / Generate JWT which will be stored within client for accessing API
            var jwt = _authTokenService.CreateToken(user, _configuration);

            // Generate session token for browser (JWT)
            var refreshToken = _authTokenService.GenerateRefreshToken();

            // Set browser cookie from the token
            _userService.SetRefreshToken(refreshToken, user);

            return jwt;
        }
    }
}
