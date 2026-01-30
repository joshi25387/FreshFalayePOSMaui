
using FreshFalaye.Pos.Shared.Abstractions;
using FreshFalaye.Pos.Shared.Data;
using FreshFalaye.Pos.Shared.Helpers;
using FreshFalaye.Pos.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;

namespace FreshFalaye.Pos.Shared.Services
{
    public class PosAuthService
    {
        private readonly ApiTokenStore _tokenStore;
        private readonly PosDbContext _db;
        private readonly ILocalAuthStore _storage;
        private readonly IHttpClientFactory _factory;

        private HttpClient? _authClient;


        public event Action? AuthStateChanged;

        public LocalUser? CurrentUser { get; private set; }
        public string? ApiToken { get; private set; }
        public bool IsInitialized { get; private set; }

        private const string USER_STORAGE_KEY = "pos_current_user";
        private const string TOKEN_STORAGE_KEY = "pos_api_token";

        //public PosAuthService(
        //                PosDbContext db,
        //                ILocalAuthStore storage,
        //                IHttpClientFactory factory,
        //                ApiTokenStore tokenStore)
        //{
        //    _db = db;
        //    _storage = storage;
        //    _tokenStore = tokenStore;   
        //    _factory = factory;
        //}


        public PosAuthService(PosDbContext db, ILocalAuthStore storage,
            IHttpClientFactory factory, ApiTokenStore tokenStore)
        {
            _db = db;
            _storage = storage;
            _factory = factory;
            _tokenStore = tokenStore;
        }
        private HttpClient AuthClient
        {
            get
            {
                if (_authClient == null)
                {
                    _authClient = _factory.CreateClient("ApiAuth");

                    if (!_authClient.DefaultRequestHeaders.Contains("X-POS-KEY"))
                    {
                        _authClient.DefaultRequestHeaders.Add("X-POS-KEY", "POS-SECRET-123");
                    }
                }
                return _authClient;
            }
        }

        public async Task<bool> GetApiToken()
        {
            CurrentUser = await _storage.GetAsync<LocalUser>(USER_STORAGE_KEY);
            // 🔐 Request API token
            var token = await RequestApiTokenAsync(CurrentUser);
            if (token == null)
                return false;
            _tokenStore.Token = token;
            ApiToken = token;
            await _storage.SetAsync(TOKEN_STORAGE_KEY, token);

            IsInitialized = true;
            NotifyStateChanged();
            return true;
        }

        /* ---------------- LOGIN ---------------- */

        public async Task<bool> LoginAsync(string username, string password)
        {
            var hash = SecurityHelper.Hash(password);

            var user = await _db.LocalUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(u =>
                    u.Username == username &&
                    u.PasswordHash == hash &&
                    u.IsActive);

            if (user == null)
                return false;

            // ✅ Local login success
            CurrentUser = user;
            await _storage.SetAsync(USER_STORAGE_KEY, user);

            // 🔐 Request API token
            //var token = await RequestApiTokenAsync(user);
            //if (token == null)
            //    return false;




            //_tokenStore.Token = token;
            //ApiToken = token;
            //await _storage.SetAsync(TOKEN_STORAGE_KEY, token);

            IsInitialized = true;
            NotifyStateChanged();
            return true;
        }

        /* ---------------- TOKEN EXCHANGE ---------------- */

        private async Task<string?> RequestApiTokenAsync(LocalUser user)
        {
            try
            {
                var response = await AuthClient.PostAsJsonAsync(
                    "api/pos/auth/token",
                    new
                    {
                        userId = user.LocalUserId,
                        username = user.Username,
                        role = user.Role,
                        branchId = 1//user.BranchId
                    });

                if (!response.IsSuccessStatusCode)
                    return null;

                var result = await response.Content
                    .ReadFromJsonAsync<TokenResponse>();

                return result?.Token;
            }
            catch
            {
                return null;
            }
        }


        public async Task RestoreApiTokenAsync()
        {
            ApiToken = await _storage.GetAsync<string>(TOKEN_STORAGE_KEY);
            _tokenStore.Token = await _storage.GetAsync<string>(TOKEN_STORAGE_KEY);

            IsInitialized = true;
            NotifyStateChanged();
        }

        /* ---------------- RESTORE SESSION ---------------- */

        public async Task RestoreUserAsync()
        {
            CurrentUser = await _storage.GetAsync<LocalUser>(USER_STORAGE_KEY);
            //ApiToken = await _storage.GetAsync<string>(TOKEN_STORAGE_KEY);
            //_tokenStore.Token = await _storage.GetAsync<string>(TOKEN_STORAGE_KEY);

            IsInitialized = true;
            NotifyStateChanged();
        }

        /* ---------------- LOGOUT ---------------- */

        public async Task LogoutAsync()
        {
            CurrentUser = null;
            ApiToken = null;
            _tokenStore.Token = null;
            await _storage.RemoveAsync(USER_STORAGE_KEY);
            await _storage.RemoveAsync(TOKEN_STORAGE_KEY);
            
            NotifyStateChanged();
        }

        /* ---------------- PIN UNLOCK ---------------- */

        public async Task<bool> UnlockWithPinAsync(string pin)
        {
            if (CurrentUser == null)
                return false;

            var hash = SecurityHelper.Hash(pin);
            if (CurrentUser.PinHash != hash)
                return false;

            NotifyStateChanged();
            return true;
        }

        private void NotifyStateChanged()
        {
            AuthStateChanged?.Invoke();
        }

        private class TokenResponse
        {
            public string Token { get; set; } = "";
        }
    }
}
