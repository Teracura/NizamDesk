using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NizamDesk.UI.Components.Pages;

public class Logout : PageModel
{
    public async Task<IActionResult> OnGet()
    {
        await HttpContext.SignOutAsync();

        return RedirectToPage("/");
    }
}