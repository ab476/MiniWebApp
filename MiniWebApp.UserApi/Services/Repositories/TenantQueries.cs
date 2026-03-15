using Microsoft.EntityFrameworkCore;
using MiniWebApp.UserApi.Domain;
using MiniWebApp.UserApi.Models.Tenants;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System;

namespace MiniWebApp.UserApi.Services.Repositories;

public sealed class TenantQueries(UserDbContext dbContext, IRequestContext requestContext)
    : RepositoryBase(requestContext), ITenantQueries
{
    public async Task<HashSet<string>> GetExistingTenantNamesAsync(CancellationToken ct = default)
    {
        return await dbContext.Tenants
            .AsNoTracking()
            .Select(t => t.Name)
            .ToHashSetAsync(StringComparer.InvariantCultureIgnoreCase, ct);
    }

    public async Task<Outcome<TenantResponse>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var tenant = await dbContext.Tenants
            .TagWith("Tenants.GetById")
            .AsNoTracking()
            .ProjectToResponse()
            .FirstOrDefaultAsync(t => t.Id == id, ct);

        return tenant is null
            ? ("Tenant not found.", StatusCodes.Status404NotFound)
            : (StatusCodes.Status200OK, tenant);
    }

    public async Task<Outcome<IReadOnlyList<TenantResponse>>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var tenants = await dbContext.Tenants.TagWith("Tenants.GetPaged").AsNoTracking().OrderByDescending(t => t.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ProjectToResponse().ToListAsync(ct);

        return (StatusCodes.Status200OK, tenants);
    }
}