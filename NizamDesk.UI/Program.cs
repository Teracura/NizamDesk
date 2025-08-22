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

builder.Services.AddGitHubAuth(builder.Configuration);

builder.Services.AddScoped<ExternalLoginManager>();
builder.Services.AddScoped<ExternalLoginService>();

var app = builder.Build();

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

app.MapGet("/login", async context =>
{
    var redirectUrl = "/";
    var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
    await context.ChallengeAsync("GitHub", properties);
});

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