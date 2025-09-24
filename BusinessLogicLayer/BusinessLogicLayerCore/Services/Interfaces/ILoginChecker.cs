namespace BusinessLogicLayerCore.Services.Interfaces
{
    public interface ILoginChecker
    {
        bool CheckCredentials(string username, string password);
    }
}
