using Newtonsoft.Json;

namespace Clay.SmartDoor.Core.Models
{
    public class ApiResponse<T>
    {
        public T Data { get; set; } = default!;
        public bool Succeeded { get; set; }
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; }

        public ApiResponse(int statusCode, bool success, string msg, T data)
        {
            Data = data;
            Succeeded = success;
            StatusCode = statusCode;
            Message = msg;
        }
        public ApiResponse() { }

        /// <summary>
        /// Sets the data to the appropriate response
        /// at run time
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static ApiResponse<T> Fail(string errorMessage, int statusCode = 404)
        {
            return new ApiResponse<T>
            {
                Succeeded = false,
                Message = errorMessage,
                StatusCode = statusCode
            };
        }
        public static ApiResponse<T> Success(string successMessage, T data, int statusCode = 200)
        {
            return new ApiResponse<T>
            {
                Succeeded = true,
                Message = successMessage,
                Data = data,
                StatusCode = statusCode
            };
        }
        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}
