using FreshFalaye.Pos.Shared.Abstractions;
using System.Text.Json;

namespace FreshFalaye.Pos.Maui.Services
{
    public class MauiAuthStore : ILocalAuthStore
    {
        public async Task SetAsync<T>(string key, T value)
        {
            var json = JsonSerializer.Serialize(value);
            await SecureStorage.SetAsync(key, json);
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var json = await SecureStorage.GetAsync(key);
            return json == null
                ? default
                : JsonSerializer.Deserialize<T>(json);
        }

        public Task RemoveAsync(string key)
        {
            SecureStorage.Remove(key);
            return Task.CompletedTask;
        }
    }
}
