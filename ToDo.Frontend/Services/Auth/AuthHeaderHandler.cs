using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;

namespace ToDo.Frontend.Services.Auth
{
    public class AuthHeaderHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorage;
        private readonly CustomAuthStateProvider _authStateProvider;
        private readonly NavigationManager _navigation;
        public AuthHeaderHandler(ILocalStorageService localStorage, CustomAuthStateProvider authStateProvider, NavigationManager navigation)
        {
            _localStorage = localStorage;
            _authStateProvider = authStateProvider;
            _navigation = navigation;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken ct)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");

            if (!string.IsNullOrWhiteSpace(token))
            {
                var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

                if (jwt.ValidTo < DateTime.UtcNow)
                {
                    await HandleInvalidToken();
                    return new HttpResponseMessage(HttpStatusCode.Unauthorized)
                    {
                        RequestMessage = request
                    };
                }

                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await base.SendAsync(request, ct);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await HandleInvalidToken();
            }

            return response;
        }

        private async Task HandleInvalidToken()
        {
            await _localStorage.RemoveItemAsync(CustomAuthStateProvider.TokenKey);
            _authStateProvider.NotifyTokenChanged(null);
            _navigation.NavigateTo("/login");
        }
    }
}
