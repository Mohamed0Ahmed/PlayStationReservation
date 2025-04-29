namespace System.Shared.Exceptions
{
    public class CustomException(string message, int statusCode = 400) : Exception(message)
    {
        public int StatusCode { get; } = statusCode;
    }
}