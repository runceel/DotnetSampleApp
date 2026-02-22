namespace DotnetSampleApp.Domain.Repositories;

using DotnetSampleApp.Domain.Entities;

public interface IAttendeeRepository
{
    Task<IReadOnlyList<Attendee>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Attendee?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(Attendee attendee, CancellationToken cancellationToken = default);
}
