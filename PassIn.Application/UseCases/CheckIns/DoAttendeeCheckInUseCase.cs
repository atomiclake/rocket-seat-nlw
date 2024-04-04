using PassIn.Communication.Responses;
using PassIn.Exceptions;
using PassIn.Infrastructure;
using PassIn.Infrastructure.Entities;

namespace PassIn.Application.UseCases.CheckIns;

public class DoAttendeeCheckInUseCase
{
    private readonly PassInDbContext _dbContext;

    public DoAttendeeCheckInUseCase()
    {
        _dbContext = new();
    }

    public ResponseResourceRegisteredJson Execute(Guid attendeeId)
    {
        Validate(attendeeId);

        CheckIn entity = new()
        {
            Created_at = DateTime.UtcNow,
            Attendee_Id = attendeeId
        };

        _dbContext.CheckIns.Add(entity);
        _dbContext.SaveChanges();

        return new()
        {
            Id = entity.Id
        };
    }

    public void Validate(Guid attendeeId)
    {
        Attendee attendee = _dbContext.Attendees.Find(attendeeId) ?? throw new NotFoundException($"Could not find attendee with id '{attendeeId}'");

        if (_dbContext.CheckIns.Any(checkIn => checkIn.Attendee_Id == attendee.Id))
        {
            throw new RequestConflictException($"The attendee with id '{attendee.Name}' is already checked in the event.");
        }
    }
}
