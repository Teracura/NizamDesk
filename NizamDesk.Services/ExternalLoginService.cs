using NizamDesk.Entities;
using NizamDesk.Entities.Users;
using NizamDesk.Logic;

namespace NizamDesk.Interfaces;

public class ExternalLoginService(ExternalLoginManager manager)
{
    public async Task<User> GetOrCreateUserAsync(ExternalUserInfo info, string contextAccessToken)
    {
        return await manager.GetOrCreateUserAsync(info, contextAccessToken);
    }
}