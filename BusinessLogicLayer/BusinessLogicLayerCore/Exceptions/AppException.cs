namespace BusinessLogicLayerCore.Exceptions;

public abstract class AppException : Exception
{
    public string ErrorCode { get; }
    public int StatusCode { get; }
    
    protected AppException(
        string message, 
        string errorCode = "GENERAL_ERROR",
        int statusCode = 500) 
        : base(message)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
    }
    
}