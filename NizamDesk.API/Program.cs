using Microsoft.EntityFrameworkCore;
using NizamDesk.API;
using NizamDesk.API.Data;
using NizamDesk.API.Data.DataManagers;
using NizamDesk.API.EndPoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<UserManager>();
builder.Services.AddScoped<CompanyManager>();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseHttpsRedirection();
await UserEndpoints.Map(app).ConfigureAwait(false);
await CompanyEndPoints.Map(app).ConfigureAwait(false);

app.Run();