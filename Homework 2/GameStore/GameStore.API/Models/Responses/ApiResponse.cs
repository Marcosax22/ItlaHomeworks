namespace GameStore.API.Models.Responses
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public int StatusCode { get; set; } = 200; 

        public static ApiResponse<T> Success(T data, int code = 200, string message = "Success")
        {
            return new ApiResponse<T>
            {
                IsSuccess = true,
                Message = message,
                Data = data,
                StatusCode = code
            };
        }

        public static ApiResponse<T> Fail(string message, int code = 400)
        {
            return new ApiResponse<T>
            {
                IsSuccess = false,
                Message = message,
                Data = default,
                StatusCode = code
            };
        }
    }
}
