

namespace BusinessLogicLayerCore.Exceptions;

public class ValidPasswordException : AppException{
    public ValidPasswordException()  
        : base(
            "Password is weak",
            "INVALID_CREDENTIALS",
            406 
        ) {}
}