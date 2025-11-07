using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NizamDesk.API.Data;
using NizamDesk.API.Services;

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

        var connectionString = config.GetConnectionString("DefaultConnection")!;
        
        services.AddDbContextFactory<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<UserService>();
        services.AddScoped<PasswordService>();

        _serviceProvider = services.BuildServiceProvider();
    }

    protected T GetService<T>() => _serviceProvider.GetRequiredService<T>();
}