namespace DotnetSampleApp.Application.UseCases;

using DotnetSampleApp.Application.Exceptions;
using DotnetSampleApp.Application.Interfaces;
using DotnetSampleApp.Domain.Entities;
using DotnetSampleApp.Domain.Repositories;

public class UpdateAttendeeAttendanceUseCase(IAttendeeRepository repository, IUnitOfWork unitOfWork)
{
    public async Task ExecuteAsync(int attendeeId, bool isAttended, CancellationToken cancellationToken = default)
    {
        var attendee = await repository.GetByIdAsync(attendeeId, cancellationToken)
            ?? throw new NotFoundException(nameof(Attendee), attendeeId);

        if (isAttended)
            attendee.MarkAsAttended();
        else
            attendee.MarkAsNotAttended();

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
