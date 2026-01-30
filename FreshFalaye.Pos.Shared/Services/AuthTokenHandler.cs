using FreshFalaye.Pos.Shared.Models;
using System.Net.Http.Headers;

namespace FreshFalaye.Pos.Shared.Services
{
    public class AuthTokenHandler : DelegatingHandler
    {
        private readonly ApiTokenStore _tokenStore;

        public AuthTokenHandler(ApiTokenStore tokenStore)
        {
            _tokenStore = tokenStore;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var token = _tokenStore.Token;

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }


}
