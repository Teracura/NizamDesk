using NizamDesk.Entities;
using NizamDesk.Entities.Users;
using NizamDesk.Logic;

namespace NizamDesk.Services;

public class ExternalLoginService : IExternalLoginService
{
    private readonly ExternalLoginManager _manager;

    public ExternalLoginService(ExternalLoginManager manager)
    {
        _manager = manager;
    }

    public async Task<User> GetOrCreateUserAsync(ExternalUserInfo info, string accessToken)
    {
        return await _manager.GetOrCreateUserAsync(info, accessToken);
    }

    public async Task<bool> UserExistsAsync(string provider, string providerId)
    {
        var user = await _manager.GetUserByExternalLoginAsync(provider, providerId);
        return user != null;
    }

    public async Task<User?> GetUserByExternalLoginAsync(string provider, string providerId)
    {
        return await _manager.GetUserByExternalLoginAsync(provider, providerId);
    }
}