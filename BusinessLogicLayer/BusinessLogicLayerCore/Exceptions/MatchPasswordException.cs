namespace BusinessLogicLayerCore.Exceptions;

public class MatchPasswordException : AppException{
    public MatchPasswordException()  
        : base(
            "Passwords do not match",
            "INVALID_CREDENTIALS",
            406 
        ) {}
}