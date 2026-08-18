using FinTrack.Server.Models;

namespace FinTrack.Server.Services.Auth;

public interface IJwtTokenGenerator
{
    (string token, DateTime expiresAt) GenerateToken(ApplicationUser user);
    string GenerateRefreshToken();
}
