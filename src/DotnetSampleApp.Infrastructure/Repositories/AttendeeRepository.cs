namespace DotnetSampleApp.Infrastructure.Repositories;

using DotnetSampleApp.Domain.Entities;
using DotnetSampleApp.Domain.Repositories;
using DotnetSampleApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class AttendeeRepository(AppDbContext dbContext) : IAttendeeRepository
{
    public async Task<IReadOnlyList<Attendee>> GetAllAsync(CancellationToken cancellationToken = default)
        => await dbContext.Attendees.ToListAsync(cancellationToken);

    public async Task<Attendee?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await dbContext.Attendees
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task AddAsync(Attendee attendee, CancellationToken cancellationToken = default)
        => await dbContext.Attendees.AddAsync(attendee, cancellationToken);
}
