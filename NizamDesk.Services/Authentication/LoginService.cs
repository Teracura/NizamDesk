using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NizamDesk.UI;
using Teracura.TestingWebApp.Entities;

namespace Teracura.TestingWebApp.Interfaces.Authentication;

public class LoginService(UserService userService, PasswordServices passwordService)
{
    public async Task<ClaimsPrincipal?> LoginInternalAsyncHttp(LoginModel login, HttpContext context)
    {
        var email = login.Email?.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(login.Password))
            return null;

        var user = await userService.GetUserAsyncIfNotInternal(email);
        if (user == null) return null;

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Email, user.Email)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        // Set cookie
        await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return principal;
    }

    public static void ConfigureExternalProviders(AuthenticationBuilder builder, IConfiguration config)
    {
        var providers = config.GetSection("OAuthClients")
            .Get<List<OAuthClientConfiguration>>() ?? [];

        foreach (var provider in providers)
        {
            builder.AddOAuth(provider.Name, options =>
            {
                options.ClientId = provider.ClientId;
                options.ClientSecret = provider.ClientSecret;
                options.CallbackPath = new PathString(provider.CallbackPath);
                options.AuthorizationEndpoint = provider.AuthorizationEndpoint;
                options.TokenEndpoint = provider.TokenEndpoint;
                options.UserInformationEndpoint = provider.UserInformationEndpoint;
                options.SaveTokens = true;

                if (provider.Scopes != null)
                    foreach (var scope in provider.Scopes)
                        options.Scope.Add(scope);

                options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, provider.IdClaimKey);
                options.ClaimActions.MapJsonKey(ClaimTypes.Name, provider.NameClaimKey);
                options.ClaimActions.MapJsonKey(ClaimTypes.Email, provider.EmailClaimKey);

                options.Events.OnCreatingTicket = async context =>
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                    request.Headers.Accept.Add(
                        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    request.Headers.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", context.AccessToken);

                    var response = await context.Backchannel.SendAsync(request);
                    response.EnsureSuccessStatusCode();

                    using var userDoc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                    context.RunClaimActions(userDoc.RootElement);

                    var email = context.Principal!.FindFirstValue(ClaimTypes.Email);
                    if (string.IsNullOrEmpty(email))
                        throw new InvalidOperationException("Email is required to create a user.");

                    // Resolve UserServices from DI
                    var userService = context.HttpContext.RequestServices.GetRequiredService<UserService>();

                    var externalInfo = new ExternalUserInfo(
                        Provider: context.Scheme.Name,
                        ProviderId: context.Principal!.FindFirstValue(ClaimTypes.NameIdentifier)!,
                        Name: context.Principal!.FindFirstValue(ClaimTypes.Name) ?? "unknown user",
                        Email: email,
                        AccessToken: context.AccessToken
                    );

                    var user = await userService.ConnectExternalAccount(externalInfo);

                    var identity = (ClaimsIdentity)context.Principal!.Identity!;
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
                    identity.AddClaim(new Claim(ClaimTypes.Name, user.Name));
                    identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
                };
            });
        }
    }
}
