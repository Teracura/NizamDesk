using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using NizamDesk.API;
using NizamDesk.API.Cryptography;
using NizamDesk.API.Data;
using NizamDesk.API.Data.DataManagers;
using NizamDesk.UI;
using Teracura.TestingWebApp.Interfaces;
using App = NizamDesk.UI.Components.App;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// builder.Services.AddDbContextFactory<AppDbContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

var authBuilder = builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme);
// LoginService.ConfigureExternalProviders(authBuilder,builder.Configuration);

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAntiforgery();
// builder.Services.AddHttpClient<UserService>(client =>
// {
//     client.BaseAddress = new Uri("https://localhost:7209");
// });

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<PasswordManager>();
builder.Services.AddScoped<PasswordServices>();
builder.Services.AddScoped<UserManager>();
// builder.Services.AddScoped<UserService>();
// builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<EmailService>();

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

// app.MapGet("/login/{provider}", async (HttpContext context, string provider, LoginService loginService) =>
// {
//     if (string.IsNullOrEmpty(provider))
//     {
//         context.Response.StatusCode = 400;
//         await context.Response.WriteAsync("Provider not specified");
//         return;
//     }
//
//     if (provider.ToLowerInvariant() == "internal")
//     {
//         // Get login info from query parameters
//         var email = context.Request.Query["email"].ToString();
//         var password = context.Request.Query["password"].ToString();
//         if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
//         {
//             context.Response.StatusCode = 400;
//             await context.Response.WriteAsync("Email or password missing");
//             return;
//         }
//
//         var loginModel = new LoginModel
//         {
//             Email = email,
//             Password = password
//         };
//
//         var principal = await loginService.LoginInternalAsyncHttp(loginModel, context);
//         if (principal == null)
//         {
//             context.Response.StatusCode = 401;
//             await context.Response.WriteAsync("Invalid email or password");
//             return;
//         }
//
//         context.Response.Redirect("/"); // login successful
//         return;
//     }
//     // OAuth external provider
//     var properties = new AuthenticationProperties { RedirectUri = "/" };
//     await context.ChallengeAsync(provider, properties);
// });


app.MapGet("/logout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/");
});

// app.MapPost("/internal-login", async (HttpContext context, LoginModel login, LoginService loginService) =>
// {
//     // Validate and sign in user
//     var principal = await loginService.LoginInternalAsyncHttp(login, context);
//     if (principal == null)
//     {
//         context.Response.StatusCode = StatusCodes.Status401Unauthorized;
//         await context.Response.WriteAsync("Invalid email or password");
//         return;
//     }
//
//     // Redirect to home page (or wherever)
//     context.Response.Redirect("/");
// });

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.Run();