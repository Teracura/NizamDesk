using Microsoft.EntityFrameworkCore;
using NizamDesk.API.Cryptography;
using NizamDesk.API.EndpointEntities.Companies;
using Teracura.TestingWebApp.Entities.Companies;
using Teracura.TestingWebApp.Entities.DataScheme.Companies;

namespace NizamDesk.API.Data.DataManagers;

public class CompanyManager(IDbContextFactory<AppDbContext> dbContextFactory)
{
    public async Task<CompanyResponse?> CreateCompanyAsync(CompanyCreateRequest request)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
        var company = await db.Companies
            .FirstOrDefaultAsync(c => c.Name == request.Name)
            .ConfigureAwait(false);
        var passPair = PasswordManager.HashPassword(request.Password);
        var newCompany = new Company
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            EntryPassword = passPair.Hash,
            EntrySalt = passPair.Salt
        };

        if (company is not null) return null;
        db.Companies.Add(newCompany);
        await db.SaveChangesAsync().ConfigureAwait(false);
        return new CompanyResponse
        {
            Id = newCompany.Id,
            Name = newCompany.Name
        };
    }

    public async Task<bool> DeleteCompanyAsync(Guid id)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
        var company = await db.Companies.FindAsync(id).ConfigureAwait(false);

        if (company is null) return false;

        db.Companies.Remove(company);
        await db.SaveChangesAsync().ConfigureAwait(false);
        return true;
    }

    public async Task<CompanyResponse?> GetCompanyAsync(Guid id)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
        var company = await db.Companies.FindAsync(id).ConfigureAwait(false);

        if (company is null) return null;
        return new CompanyResponse
        {
            Id = company.Id,
            Name = company.Name
        };
    }

    public async Task<List<CompanyResponse>> GetCompaniesAsync()
    {
        await using var db = await dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

        return await db.Companies
            .Select(c => new CompanyResponse
            {
                Id = c.Id,
                Name = c.Name
            })
            .ToListAsync()
            .ConfigureAwait(false);
    }

    public async Task<CompanyResponse?> UpdateCompanyAsync(Guid id, CompanyUpdateRequest request)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

        var currentCompany = await db.Companies.FindAsync(id).ConfigureAwait(false);

        if (request.Name is not null && currentCompany is not null)
        {
            currentCompany.Name = request.Name;
        }

        if (request.Password is not null && currentCompany is not null)
        {
            var passPair = PasswordManager.HashPassword(request.Password);
            currentCompany.EntryPassword = passPair.Hash;
            currentCompany.EntrySalt = passPair.Salt;
        }

        await db.SaveChangesAsync().ConfigureAwait(false);
        if (currentCompany is null) return null;

        return new CompanyResponse
        {
            Id = currentCompany.Id,
            Name = currentCompany.Name
        };
    }

    public async Task<CompanyJoinStatus> UserJoinCompanyAsync(Guid companyId, JoinCompanyForm form)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

        var company = await db.Companies.FirstOrDefaultAsync(c => c.Name == form.CompanyName)
            .ConfigureAwait(false);
        if (company is null) return CompanyJoinStatus.CompanyNotFound;

        var user = await db.Users.FindAsync(form.UserId).ConfigureAwait(false);
        if (user is null) return CompanyJoinStatus.UserNotFound;

        var passwordOk = PasswordManager.VerifyPassword(company.EntryPassword, company.EntrySalt, form.Password);
        if (!passwordOk || company.Id != companyId) return CompanyJoinStatus.CompanyNotFound;

        var alreadyMember = await db.CompanyMemberships
            .AnyAsync(m => m.CompanyId == companyId && m.UserId == form.UserId)
            .ConfigureAwait(false);
        if (alreadyMember) return CompanyJoinStatus.UserAlreadyMember;

        await db.CompanyMemberships.AddAsync(new CompanyMembership
        {
            CompanyId = companyId,
            UserId = form.UserId
        }).ConfigureAwait(false);

        await db.SaveChangesAsync().ConfigureAwait(false);
        return CompanyJoinStatus.Success;
    }


    public async Task<bool> UserLeaveCompanyAsync(Guid companyId, Guid userId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

        var companyExists = await db.Companies.AnyAsync(c => c.Id == companyId).ConfigureAwait(false);
        if (!companyExists) return false;

        var userExists = await db.Users.AnyAsync(u => u.Id == userId).ConfigureAwait(false);
        if (!userExists) return false;

        var membership = await db.CompanyMemberships
            .FirstOrDefaultAsync(cm => cm.CompanyId == companyId && cm.UserId == userId)
            .ConfigureAwait(false);

        if (membership is null)
            return false;

        db.CompanyMemberships.Remove(membership);
        await db.SaveChangesAsync().ConfigureAwait(false);

        return true;
    }

}