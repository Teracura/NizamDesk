using System.Text.RegularExpressions;

namespace Teracura.TestingWebApp.Interfaces;

public partial class EmailService
{
    [GeneratedRegex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$")]
    public static partial Regex EmailRegex();
    
    public bool IsValidEmail(string email) => EmailRegex().IsMatch(email);
    
}