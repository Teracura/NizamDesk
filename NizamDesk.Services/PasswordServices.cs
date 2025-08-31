using System.Text.RegularExpressions;
using Teracura.TestingWebApp.Logic.Cryptography;

namespace Teracura.TestingWebApp.Interfaces;

public partial class PasswordServices(PasswordManager passwordManager)
{
    [GeneratedRegex(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*\-]).{8,}$")]
    private static partial Regex StrongPasswordRegex();

    public (byte[] Hash, byte[] Salt) HashPassword(string password)
    {
        return passwordManager.HashPassword(password);
    }

    public void VerifyPassword(string password, Byte[] hash, Byte[] salt)
    {
        passwordManager.VerifyPassword(hash, salt, password);
    }

    public bool IsStrongPassword(string password)
    {
        var regex = StrongPasswordRegex();
        return regex.IsMatch(password);
    }
}