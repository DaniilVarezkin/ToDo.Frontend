using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using ToDo.Shared.Dto.Auth;
using ToDo.Frontend.Common.Exceptions;

namespace ToDo.Frontend.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _http;
        private readonly ILocalStorageService _localStorage;
        private readonly CustomAuthStateProvider _authStateProvider;

        public AuthService(HttpClient http, ILocalStorageService localStorage, AuthenticationStateProvider authStateProvider)
        {
            _http = http;
            _localStorage = localStorage;
            _authStateProvider = (CustomAuthStateProvider)authStateProvider;
        }

        public async Task<string> LoginAsync(LoginDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/auth/Login", dto);

            if (response.IsSuccessStatusCode)
            {
                var loginObj = await response.Content.ReadFromJsonAsync<LoginResponse>()!;
                var token = loginObj.Token!;

                await _localStorage.SetItemAsync("authToken", token);

                _authStateProvider.NotifyTokenChanged(token);
                return token;
            }
            else
            {
                ProblemDetails? problemDetails = null;
                try
                {
                    problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
                }
                catch
                {
                }

                var message = problemDetails?.Title
                              ?? $"Сервер вернул {(int)response.StatusCode} {response.ReasonPhrase}";
                throw new ServerException(message, problemDetails);
            }
        }

        public async Task RegisterAsync(RegisterDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/auth/Register", dto);
            if (response.IsSuccessStatusCode) return;

            ProblemDetails? problemDetails = null;
            try
            {
                problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            }
            catch
            {
            }

            var message = problemDetails?.Title
                          ?? $"Сервер вернул {(int)response.StatusCode} {response.ReasonPhrase}";
            throw new ServerException(message, problemDetails);
        }

        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync("authToken");
            _authStateProvider.NotifyTokenChanged(null);
        }
    }
}
