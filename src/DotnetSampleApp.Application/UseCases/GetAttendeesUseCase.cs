namespace DotnetSampleApp.Application.UseCases;

using DotnetSampleApp.Application.DTOs;
using DotnetSampleApp.Domain.Repositories;

public class GetAttendeesUseCase(IAttendeeRepository repository)
{
    public async Task<IReadOnlyList<AttendeeDto>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var attendees = await repository.GetAllAsync(cancellationToken);

        return attendees
            .Select(a => new AttendeeDto(a.Id, a.AccountName, a.IsAttended))
            .ToList();
    }
}
