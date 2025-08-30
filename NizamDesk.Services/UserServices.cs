using Teracura.TestingWebApp.Entities;
using Teracura.TestingWebApp.Entities.Users;
using Teracura.TestingWebApp.Logic;

namespace Teracura.TestingWebApp.Interfaces;

public class ExternalLoginService(UserManager users)
{
    public async Task<User> ConnectExternalAccount(ExternalUserInfo info)
    {
        return await users.LinkExternalUserAsync(info);
    }

    public async Task RegisterUser(User user)
    {
        await users.RegisterUserAsync(user);
    }

    //will be used later to discover providers for a user
    public async Task<HashSet<string>> GetAllProviders(User user)
    {
        var externalLogins = await users.GetExternalLoginsAsync(user);
        return externalLogins.Select(l => l.Provider).ToHashSet();
    }

    public async Task RemoveUserAsync(User user)
    {
        var logins = await users.GetExternalLoginsAsync(user);
        foreach (var l in logins)
        {
            users.DeleteExternalLogin(l);
        }
        users.DeleteUser(user);
        //TODO: delete all project memberships and Company memberships and User roles linked to the user
        //TODO: make all tickets assigned to the user unassigned
    }
}