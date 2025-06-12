﻿using Blazored.LocalStorage;
using System.Net.Http.Headers;

namespace ToDo.Frontend.Services.Auth
{
    public class AuthHeaderHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorage;
        public AuthHeaderHandler(ILocalStorageService localStorage) => _localStorage = localStorage;

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken ct)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrWhiteSpace(token))
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            return await base.SendAsync(request, ct);
        }
    }
}
