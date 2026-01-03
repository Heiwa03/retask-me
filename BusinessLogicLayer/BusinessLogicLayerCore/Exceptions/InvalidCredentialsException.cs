namespace BusinessLogicLayerCore.Exceptions;


public class InvalidCredentialsException : AppException
{
    public InvalidCredentialsException()
        : base(
            "Login or password are incorrect",
            "INVALID_CREDENTIALS",
            401
        ) {}
}

