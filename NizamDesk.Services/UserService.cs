// using Microsoft.AspNetCore.Authentication;
// using Microsoft.AspNetCore.Authentication.Cookies;
// using Microsoft.AspNetCore.Http;
// using NizamDesk.API;
// using Teracura.TestingWebApp.Entities;
// using Teracura.TestingWebApp.Entities.Users;
// using Teracura.TestingWebApp.Logic;
//
// namespace Teracura.TestingWebApp.Interfaces;
//
// public class UserService(UserManager userManager, IHttpContextAccessor httpContextAccessor, PasswordServices passwordServices)
// {
//     public async Task<User> ConnectExternalAccount(ExternalUserInfo info)
//     {
//         return await userManager.LinkExternalUserAsync(info);
//     }
//
//     public async Task RegisterUserAsync(User user)
//     {
//         await userManager.RegisterUserAsync(user);
//     }
//
//     public User CreateUser(string name, string email, string password)
//     {
//         var hashSalt = passwordServices.HashPassword(password);
//         return new User
//         {
//             Id = Guid.NewGuid(),
//             Name = name,
//             Email = email,
//             PasswordHash = hashSalt.Hash,
//             Salt = hashSalt.Salt
//         };
//     }
//
//     //will be used later to discover providers for a user
//     public async Task<HashSet<string>> GetAllProvidersAsync(User user)
//     {
//         var externalLogins = await userManager.GetExternalLoginsAsync(user);
//         return externalLogins.Select(l => l.Provider).ToHashSet();
//     }
//
//     public async Task RemoveUserAsync(User user)
//     {
//         var logins = await userManager.GetExternalLoginsAsync(user);
//         foreach (var l in logins)
//         {
//             userManager.DeleteExternalLogin(l);
//         }
//
//         userManager.DeleteUser(user);
//         //TODO: delete all project memberships and Company memberships and User roles linked to the user
//         //TODO: make all tickets assigned to the user unassigned
//         await LogoutAsync();
//     }
//
//     public async Task LogoutAsync()
//     {
//         if (httpContextAccessor.HttpContext != null)
//             await httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
//     }
//
//     public async Task<bool> EmailExistsAsync(string email)
//     {
//         return await userManager.EmailExistsAsync(email);
//     }
//
//     public async Task<bool> EmailExistsExternalOnlyAsync(string email)
//     {
//         return !await userManager.EmailExistsAsync(email) || await userManager.InternalLoginExistsAsync(email);
//     }
//
//     public async Task<bool> EmailExistsExternalOnlyAsync(User user)
//     {
//         return await userManager.EmailExistsAsync(user.Email) || !await userManager.InternalLoginExistsAsync(user.Email);
//     }
//     
//     public async Task<User?> GetUserAsync(string email)
//     {
//         return await userManager.GetUserAsync(email);
//     }
//
//     public async Task<User?> GetUserAsyncIfNotInternal(string email)
//     {
//         return await userManager.GetUserAsyncIfNotInternal(email);
//     }
// }