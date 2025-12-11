using System.Security.Cryptography;
using System.Text;

namespace TechChallenge.Domain.Helpers;

public static class PasswordHelper
{
    public static string HashPassword(string password, string salt)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(password + salt)));

    public static (string hash, string salt) CreateHashPassword(string password)
    {
        var buffer = new byte[16];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(buffer);
        var salt = Convert.ToBase64String(buffer);

        var hashBase64 = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(password + salt)));

        return (hashBase64, salt);
    }

    public static bool ValidatePassword(string password, string storedHash, string storedSalt)
    {
        var newHash = HashPassword(password, storedSalt);
        return newHash.Equals(storedHash, StringComparison.OrdinalIgnoreCase);
    }
}
