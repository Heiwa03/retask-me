namespace BusinessLogicLayerCore.Exceptions;

public class AccountLockedException : AppException{
    public AccountLockedException() 
        : base("ACCOUNT_LOCKED") { }
}