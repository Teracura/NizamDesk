using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.OAuth;
using Teracura.TestingWebApp.Entities;

namespace Teracura.TestingWebApp.Interfaces.Authentication;

public static class AuthDataCollectionService
{
    public static AuthenticationBuilder AddGitHubAuth(this IServiceCollection services, IConfiguration config)
    {
        return services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "GitHub";
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            })
            .AddOAuth("GitHub", options =>
            {
                options.ClientId = config["Authentication:GitHub:ClientId"]!;
                options.ClientSecret = config["Authentication:GitHub:ClientSecret"]!;
                options.CallbackPath = new PathString("/signin-github");

                options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
                options.TokenEndpoint = "https://github.com/login/oauth/access_token";
                options.UserInformationEndpoint = "https://api.github.com/user";

                options.SaveTokens = true;

                options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
                options.ClaimActions.MapJsonKey(ClaimTypes.Name, "login");
                options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
                
                options.Scope.Add("user:email");

                options.Events = new OAuthEvents
                {
                    OnCreatingTicket = async context =>
                    {
                        // fetch from GitHub
                        var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", context.AccessToken);
                        request.Headers.UserAgent.ParseAdd("TeracuraApp");

                        var response = await context.Backchannel.SendAsync(request);
                        response.EnsureSuccessStatusCode();

                        using var userDoc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
                        var root = userDoc.RootElement;

                        var info = new ExternalUserInfo(
                            Provider: context.Scheme.Name, // "GitHub"
                            ProviderId: root.GetProperty("id").GetInt64().ToString(),
                            Name: root.GetProperty("login").GetString(),
                            Email: root.TryGetProperty("email", out var emailProp) ? emailProp.GetString() : null,
                            AccessToken: context.AccessToken
                        );

                        var loginService = context.HttpContext.RequestServices.GetRequiredService<ExternalLoginService>();
                        var user = await loginService.GetOrCreateUserAsync(info);

                        var identity = (ClaimsIdentity)context.Principal?.Identity!;
                        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
                        identity.AddClaim(new Claim(ClaimTypes.Name, user.Name));
                        if (!string.IsNullOrEmpty(user.Email))
                            identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
                    }
                };
            });
    }
}