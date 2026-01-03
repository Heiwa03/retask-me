namespace BusinessLogicLayerCore.Exceptions;

public class UserNotFoundException : AppException
{
    public UserNotFoundException()  // TODO: Delete later
        : base("USER_NOT_FOUND") { }
}

