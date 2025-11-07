using System.Text.RegularExpressions;
using NizamDesk.API.Cryptography;

namespace Teracura.TestingWebApp.Interfaces;

public partial class PasswordServices(PasswordManager passwordManager)
{
    [GeneratedRegex(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*\-]).{8,}$")]
    private static partial Regex StrongPasswordRegex();

    public (byte[] Hash, byte[] Salt) HashPassword(string password)
    {
        return PasswordManager.HashPassword(password);
    }

    public bool VerifyPassword(string password, byte[] hash, byte[] salt)
    {
        return PasswordManager.VerifyPassword(hash, salt, password);
    }

    public bool IsStrongPassword(string password)
    {
        var regex = StrongPasswordRegex();
        return regex.IsMatch(password);
    }
}