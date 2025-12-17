using Microsoft.EntityFrameworkCore;
using NizamDesk.API;
using NizamDesk.API.Data;
using NizamDesk.API.EndPoints;
using NizamDesk.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<CompanyService>();
builder.Services.AddScoped<RoleService>();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseHttpsRedirection();
await UserEndpoints.Map(app).ConfigureAwait(false);
await CompanyEndPoints.Map(app).ConfigureAwait(false);

app.Run();

public partial class Program { }