using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Teracura.TestingWebApp.Interfaces;
using Teracura.TestingWebApp.Interfaces.Authentication;
using Teracura.TestingWebApp.Logic;
using Teracura.TestingWebApp.Logic.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<ExternalLoginManager>();
builder.Services.AddScoped<ExternalLoginService>();

builder.Services.AddOAuthProviders(builder.Configuration);

var app = builder.Build();

app.MapGet("/login/{provider}", async (HttpContext context, string provider) =>
{
    if (string.IsNullOrEmpty(provider))
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsync("Provider not specified");
        return;
    }

    var properties = new AuthenticationProperties { RedirectUri = "/" };
    await context.ChallengeAsync(provider, properties);
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapGet("/logout", async context =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    context.Response.Redirect("/");
});

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();