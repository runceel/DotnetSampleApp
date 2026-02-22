namespace DotnetSampleApp.Infrastructure.Persistence;

using DotnetSampleApp.Application.Interfaces;

public class UnitOfWork(AppDbContext dbContext) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await dbContext.SaveChangesAsync(cancellationToken);
}
