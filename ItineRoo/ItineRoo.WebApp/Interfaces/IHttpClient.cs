using ItineRoo.Dto;

namespace ItineRoo.WebApp.Interfaces
{
    public interface IHttpClient
    {
        Task<T?> Get<T>(string endpoint, string accessToken);
        Task<ApiResponse<U?>> PostRequest<T, U>(string endpoint, T model);
    }
}
