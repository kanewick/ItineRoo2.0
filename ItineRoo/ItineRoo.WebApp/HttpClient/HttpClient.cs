using ItineRoo.Dto;
using ItineRoo.WebApp.Interfaces;
using ItineRoo.WebApp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ItineRoo.WebApp.HttpClient
{
    public class HttpClient : IHttpClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HttpClient(IHttpClientFactory httpClientFactory) =>
        _httpClientFactory = httpClientFactory;

        public async Task<T?> Get<T>(string endpoint, string accessToken)
        {
            var httpClient = _httpClientFactory.CreateClient("Api");

            if (!string.IsNullOrEmpty(accessToken))
            {
                httpClient.DefaultRequestHeaders.Authorization
                = new System.Net.Http.Headers.AuthenticationHeaderValue($"Bearer", $"{accessToken}");
            }


            var content = await httpClient.GetAsync(endpoint);

            if (content.IsSuccessStatusCode)
            {
                using var contentStream =
                    await content.Content.ReadAsStreamAsync();

                return await JsonSerializer.DeserializeAsync
                    <T>(contentStream, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            }

            return default;
        }

        public async Task<ApiResponse<U?>> PostRequest<T, U>(string endpoint, T model)
        {
            var result = new ApiResponse<U?>();

            var httpClient = _httpClientFactory.CreateClient("Api");
            var content = await httpClient.PostAsJsonAsync<T>(endpoint, model);

            if (content.IsSuccessStatusCode)
            {
                using var contentStream =
                    await content.Content.ReadAsStreamAsync();

                result = await JsonSerializer.DeserializeAsync
                    <ApiResponse<U?>>(contentStream, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? new ApiResponse<U?>
                    {
                        HttpStatusCode = content.StatusCode,
                        Message = "Something went wrong!!",
                        Result = default(U?)
                    };

            }

            return result;
        }
    }
}
