namespace BarrioBook.Application.Responses
{
    public class ApiResponse<T>
    {

        public bool IsSuccess { get; set; }

        public string Message { get; set; } = string.Empty;

        public T? Data { get; set; }

        public int StatusCode { get; set; } = 200;

        public ApiResponse(bool isSuccess, T? data, string? message = null, int statusCode = 200)
        {
            IsSuccess = isSuccess;
            Data = data;
            Message = message ?? string.Empty;
            StatusCode = statusCode;
        }

        public ApiResponse()
        {
        }

        public static ApiResponse<T> Success(T data, int statusCode = 200, string message = "Success")
        {
            return new ApiResponse<T>(true, data, message, statusCode);
        }

        public static ApiResponse<T> Fail(string message, int statusCode = 400)
        {
            return new ApiResponse<T>(false, default, message, statusCode);
        }
    }
}
