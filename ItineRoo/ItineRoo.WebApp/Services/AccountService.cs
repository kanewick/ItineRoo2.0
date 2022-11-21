using ItineRoo.Dto;
using ItineRoo.WebAPI.Models;
using ItineRoo.WebApp.Interfaces;
using ItineRoo.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace ItineRoo.WebApp.Services
{
    public class AccountService : IAccountService
    {
        private readonly IHttpClient _httpClient;

        public AccountService(IHttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<ApiResponse<UserRegisterModel?>> RegisterAccount(UserRegisterModel userRegisterModel)
        {
            return await _httpClient
                .PostRequest<UserRegisterModel, UserRegisterModel?>("api/User/register", userRegisterModel);
        }

        public async Task<ApiResponse<string>> LoginAccount(UserLoginModel userLoginModel)
        {
            // Login to the API - this also creates the authtoken
            var result = await _httpClient
                .PostRequest<UserLoginModel, string>("api/User/login", userLoginModel);

            if (result.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                return new ApiResponse<string>
                {
                    HttpStatusCode = System.Net.HttpStatusCode.BadRequest,
                    Message = result.Message,
                    Result = result.Result
                };
            }

            // Otherwise find the user and get the authtoken and create

            return result;
        }

        public async Task<ApiResponse<UserVerificationModel?>> VerifyUser(UserVerificationModel userVerificationModel)
        {
            return await _httpClient
                .PostRequest<string, UserVerificationModel?>("api/User/verify", userVerificationModel.Token);
        }
    }
}
