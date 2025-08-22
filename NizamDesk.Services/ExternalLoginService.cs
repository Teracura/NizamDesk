using Teracura.TestingWebApp.Entities;
using Teracura.TestingWebApp.Entities.Users;
using Teracura.TestingWebApp.Logic;

namespace Teracura.TestingWebApp.Interfaces;

public class ExternalLoginService(ExternalLoginManager manager)
{
    public async Task<User> GetOrCreateUserAsync(ExternalUserInfo info)
    {
        return await manager.GetOrCreateUserAsync(info, info.AccessToken!);
    }
}