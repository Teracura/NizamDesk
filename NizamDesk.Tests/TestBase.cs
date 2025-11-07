using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NizamDesk.API.Cryptography;
using NizamDesk.API.Data.DataManagers;

namespace Teracura.TestingWebApp.Tests;

public class TestBase
{
    private readonly IServiceProvider _serviceProvider;

    public TestBase()
    {
        var services = new ServiceCollection();

        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("testsettings.json", optional: false)
            .Build();

        string connectionString = config.GetConnectionString("DefaultConnection")!;
        
        //TODO: Implement new database
        
        // services.AddDbContextFactory<AppDbContext>(options =>
        //     options.UseSqlServer(connectionString));

        services.AddScoped<UserManager>();
        services.AddScoped<PasswordManager>();

        _serviceProvider = services.BuildServiceProvider();
    }

    protected T GetService<T>() => _serviceProvider.GetRequiredService<T>();
}