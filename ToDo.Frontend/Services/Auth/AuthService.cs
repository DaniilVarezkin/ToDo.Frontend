using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using ToDo.Shared.Dto.Auth;

namespace ToDo.Frontend.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _http;
        private readonly ILocalStorageService _localStorage;
        private readonly CustomAuthStateProvider _authStateProvider;

        public AuthService(HttpClient http, ILocalStorageService localStorage , AuthenticationStateProvider authStateProvider)
        {
            _http = http;
            _localStorage = localStorage;
            _authStateProvider = (CustomAuthStateProvider)authStateProvider;
        }

        public async Task<string> LoginAsync(LoginDto dto)
        {
            // Отправляем POST /api/auth/login с телом { email, password }
            var response = await _http.PostAsJsonAsync("api/auth/Login", dto);

            response.EnsureSuccessStatusCode();

            var loginObj = await response.Content.ReadFromJsonAsync<LoginResponse>()!;
            var token = loginObj.Token;

            // Сохраняем токен в LocalStorage под ключом "authToken"
            await _localStorage.SetItemAsync("authToken", token);

            _authStateProvider.NotifyTokenChanged(token);

            return token;
        }

        public async Task<string> RegisterAsync(RegisterDto dto)
        {
            // POST /api/auth/register
            var response = await _http.PostAsJsonAsync("api/auth/Register", dto);
            response.EnsureSuccessStatusCode();

            // После успешной регистрации обычно не присылают токен,
            // но чтобы сигнатура соответствовала IAuthService, возвращаем пустую строку
            return string.Empty;
        }

        public async Task<string> LogoutAsync()
        {
            await _localStorage.RemoveItemAsync("authToken");
            _authStateProvider.NotifyTokenChanged(null);
            return string.Empty;
        }
    }
}
