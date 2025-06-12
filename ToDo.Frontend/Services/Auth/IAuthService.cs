using ToDo.Shared.Dto.Auth;

namespace ToDo.Frontend.Services.Auth
{
    public interface IAuthService
    {
        public Task<string> LoginAsync(LoginDto dto);
        public Task<string> RegisterAsync(RegisterDto dto);
        public Task<string> LogoutAsync();
    }
}
