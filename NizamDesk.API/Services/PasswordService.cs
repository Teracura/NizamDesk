using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace NizamDesk.API.Services;

public partial class PasswordService
{
    [GeneratedRegex(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*\-]).{8,}$")]
    private static partial Regex StrongPasswordRegex();
    public static (byte[] Hash, byte[] Salt) HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var pepper = (char)RandomNumberGenerator.GetInt32(fromInclusive: 100, toExclusive: 128);
        password = $"{pepper}{password}{pepper}{pepper}";
        using var deriveBytes = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
        var hash = deriveBytes.GetBytes(32);
        return (hash, salt);
    }

    public static bool VerifyPassword(byte[]? hash, byte[]? salt, string password)
    {
        if (hash == null || salt == null)
        {
            return false;
        }

        var pepper = (char)100;
        while (pepper < 128)
        {
            var pepperedPassword = $"{pepper}{password}{pepper}{pepper}";
            using var deriveBytes = new Rfc2898DeriveBytes(pepperedPassword, salt, 100_000, HashAlgorithmName.SHA256);
            var computedHash = deriveBytes.GetBytes(32);
            if (computedHash.SequenceEqual(hash))
            {
                return true;
            }
            pepper++;
        }

        return false;
    }

    public bool IsStrongPassword(string password)
    {
        var regex = StrongPasswordRegex();
        return regex.IsMatch(password);
    }
}