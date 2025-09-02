namespace NizamDesk.UI;

public class RegisterModel
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? RepeatPassword { get; set; }
    public bool AcceptTerms { get; set; }
}