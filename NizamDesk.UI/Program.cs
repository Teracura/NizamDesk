using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Teracura.TestingWebApp.Interfaces;
using Teracura.TestingWebApp.Interfaces.Authentication;
using Teracura.TestingWebApp.Logic.Data;
using Teracura.TestingWebApp.Logic;
using Teracura.TestingWebApp.Logic.Cryptography;
using App = NizamDesk.UI.Components.App;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "NizamDeskAuth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
    });
builder.Services.AddHttpClient<UserServices>(client => { client.BaseAddress = new Uri("https://localhost:7209"); });

var authBuilder = builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme);
authBuilder.AddOAuthProviders(builder.Configuration);

builder.Services.AddScoped<PasswordManager>();
builder.Services.AddScoped<PasswordServices>();
builder.Services.AddScoped<UserManager>();
builder.Services.AddScoped<UserServices>();

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAntiforgery();
builder.Services.AddHttpClient();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

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

app.MapGet("/logout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/");
});


app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.Run();