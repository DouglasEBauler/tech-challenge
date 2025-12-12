namespace TechChallenge.Domain.Helpers;

public static class PasswordHelper
{
    public static string CreateHashPassword(string password)
        => BCrypt.Net.BCrypt.HashPassword(password);

    public static bool ValidatePassword(string password, string storedHash)
        => BCrypt.Net.BCrypt.Verify(password, storedHash);
}
