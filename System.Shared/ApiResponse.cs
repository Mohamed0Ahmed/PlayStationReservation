namespace System.Shared
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public ApiResponse(T data, string message = "Success", int statusCode = 200)
        {
            IsSuccess = true;
            StatusCode = statusCode;
            Message = message;
            Data = data;
        }

        public ApiResponse(string errorMessage, int statusCode = 400)
        {
            IsSuccess = false;
            StatusCode = statusCode;
            Message = errorMessage;
            Data = default;
        }
    }
}