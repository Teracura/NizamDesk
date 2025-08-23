using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Teracura.TestingWebApp.Entities;

namespace Teracura.TestingWebApp.Interfaces.Authentication;

public static class AuthDataCollectionService
{
    public static AuthenticationBuilder AddOAuthProviders(this IServiceCollection services, IConfiguration config)
    {
        var providers = config.GetSection("Authentication:Providers")
                              .Get<List<OAuthClientConfiguration>>() ?? new List<OAuthClientConfiguration>();

        var builder = services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
        {
            options.Cookie.SameSite = SameSiteMode.None;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        });

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
                    // Build request
                    var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                    request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    request.Headers.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", context.AccessToken);

                    if (provider.Name.Equals("GitHub", StringComparison.OrdinalIgnoreCase))
                        request.Headers.UserAgent.ParseAdd("TeracuraApp");

                    var response = await context.Backchannel.SendAsync(request);
                    response.EnsureSuccessStatusCode();

                    using var userDoc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                    context.RunClaimActions(userDoc.RootElement);

                    // Use your login service to map external info to app user
                    var loginService = context.HttpContext.RequestServices.GetRequiredService<ExternalLoginService>();

                    var info = new ExternalUserInfo(
                        Provider: context.Scheme.Name,
                        ProviderId: context.Principal!.FindFirst(ClaimTypes.NameIdentifier)!.Value,
                        Name: context.Principal.FindFirst(ClaimTypes.Name)!.Value,
                        Email: context.Principal.FindFirst(ClaimTypes.Email)?.Value,
                        AccessToken: context.AccessToken
                    );

                    var user = await loginService.GetOrCreateUserAsync(info);

                    // Replace claims identity with your app's user info
                    var identity = (ClaimsIdentity)context.Principal!.Identity!;
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
                    identity.AddClaim(new Claim(ClaimTypes.Name, user.Name));
                    if (!string.IsNullOrEmpty(user.Email))
                        identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
                };
            });
        }

        return builder;
    }
}
