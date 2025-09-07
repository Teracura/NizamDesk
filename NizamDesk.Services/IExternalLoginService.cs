using NizamDesk.Entities;
using NizamDesk.Entities.Users;

namespace NizamDesk.Services;

public interface IExternalLoginService
{
    Task<User> GetOrCreateUserAsync(ExternalUserInfo info, string accessToken);
    Task<bool> UserExistsAsync(string provider, string providerId);
    Task<User?> GetUserByExternalLoginAsync(string provider, string providerId);
}