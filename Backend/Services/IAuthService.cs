using KingOfKings.Backend.DTOs;

namespace KingOfKings.Backend.Services;

public interface IAuthService
{
    Task<AuthResponse> Login(LoginRequest request);
    Task<AuthResponse> Register(RegisterRequest request);
}
