using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ToDo.Frontend.Services.Auth
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _storage;
        public const string TokenKey = "authToken";

        public CustomAuthStateProvider(ILocalStorageService storage)
            => _storage = storage;

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var savedToken = await _storage.GetItemAsync<string>(TokenKey);
            if (string.IsNullOrWhiteSpace(savedToken))
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwt;
            try
            {
                jwt = handler.ReadJwtToken(savedToken);
            }
            catch
            {
                // Невалидный токен — сбрасываем
                await _storage.RemoveItemAsync(TokenKey);
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var identity = new ClaimsIdentity(jwt.Claims, "jwt");
            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }

        /// <summary>
        /// Вызывается после успешного логина или логаута,
        /// чтобы уведомить Blazor о смене состояния.
        /// </summary>
        public void NotifyTokenChanged(string? token)
        {
            Task<AuthenticationState> authState = token switch
            {
                null => Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()))),
                _ => Task.FromResult(BuildAuthenticationState(token))
            };

            NotifyAuthenticationStateChanged(authState);
        }

        private AuthenticationState BuildAuthenticationState(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var identity = new ClaimsIdentity(jwt.Claims, "jwt");
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
    }
}
