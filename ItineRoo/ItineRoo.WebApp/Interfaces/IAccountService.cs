using ItineRoo.Dto;
using ItineRoo.WebApp.Models;

namespace ItineRoo.WebApp.Interfaces
{
    public interface IAccountService
    {
        Task<ApiResponse<string>> LoginAccount(UserLoginModel userLoginModel);
        Task<ApiResponse<UserRegisterModel?>> RegisterAccount(UserRegisterModel userRegisterModel);
        Task<ApiResponse<UserVerificationModel?>> VerifyUser(UserVerificationModel userVerificationModel);
    }
}
