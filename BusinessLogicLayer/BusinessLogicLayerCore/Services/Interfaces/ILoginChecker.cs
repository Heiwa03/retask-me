namespace BusinessLogicLayerCore.Services.Interfaces
{
    public interface ILoginChecker
    {
        Task<bool> CheckCredentials(string username, string password);
    }
}
