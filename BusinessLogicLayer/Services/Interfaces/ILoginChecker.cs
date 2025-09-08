namespace BusinessLogicLayer.Services.Interfaces
{
    public interface ILoginChecker
    {
        bool CheckCredentials(string username, string password);
    }
}
