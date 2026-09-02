using System.Security.Cryptography;
using System.Text;

namespace FinTrack.Server.Services.Auth;

public static class RefreshTokenHelper
{
    public static string HashToken(string rawToken)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
        return Convert.ToBase64String(bytes);
    }

    public static bool VerifyToken(string rawToken, string hashedToken)
    {
        var hash = HashToken(rawToken);
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(hash),
            Encoding.UTF8.GetBytes(hashedToken));
    }
}
