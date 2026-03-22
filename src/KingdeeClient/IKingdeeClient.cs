namespace KingdeeClient
{
    public interface IKingdeeClient
    {
        Task<string> ExcuteAsync<T>(string method, object[] parameters);
    }
}
