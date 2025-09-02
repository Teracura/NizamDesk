using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Teracura.TestingWebApp.Logic;
using Teracura.TestingWebApp.Logic.Cryptography;
using Teracura.TestingWebApp.Logic.Data;

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

        services.AddDbContextFactory<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<UserManager>();
        services.AddScoped<PasswordManager>();

        _serviceProvider = services.BuildServiceProvider();
    }

    protected T GetService<T>() => _serviceProvider.GetRequiredService<T>();
}