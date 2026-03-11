namespace BusinessLogicLayerCore.Exceptions;

public class UsernameExistException : AppException{
    public UsernameExistException()  // TODO: Maybe has potential vulnerability
        : base(
            "Username already exists",
            "INVALID_CREDENTIALS",
            406 // To correct error code
        ) {}
}