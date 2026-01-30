namespace FreshFalaye.Pos.Shared.Abstractions
{
    public interface ILocalAuthStore
    {
        Task SetAsync<T>(string key, T value);
        Task<T?> GetAsync<T>(string key);
        Task RemoveAsync(string key);
    }
}
