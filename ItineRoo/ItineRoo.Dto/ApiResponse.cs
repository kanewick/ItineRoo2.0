using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ItineRoo.Dto
{
    public class ApiResponse<U>
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public U? Result { get; set; }
    }

    public static class WebApiResponse
    {
        private static IActionResult MyResult<U>(int statusCode, ApiResponse<U> result)
        {
            var jsonResult = new JsonResult(result);
            jsonResult.StatusCode = statusCode;
            return jsonResult;
        }

        public static IActionResult Success<T>(T data)
        {
            return MyResult(200, new ApiResponse<T>
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Success",
                Result = data
            });
        }

        public static IActionResult Error<T>(string errorMessage)
        {
            return MyResult(400, new ApiResponse<T?>
            {
                HttpStatusCode = HttpStatusCode.BadRequest,
                Message = errorMessage,
                Result = default(T)
            });
        }
    }
}