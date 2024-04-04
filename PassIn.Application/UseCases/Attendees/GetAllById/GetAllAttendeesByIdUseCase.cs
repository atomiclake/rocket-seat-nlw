using Microsoft.EntityFrameworkCore;

using PassIn.Communication.Responses;
using PassIn.Exceptions;
using PassIn.Infrastructure;
using PassIn.Infrastructure.Entities;

namespace PassIn.Application.UseCases.Attendees.GetAllById;

public class GetAllAttendeesByIdUseCase
{
    private readonly PassInDbContext _dbContext;

    public GetAllAttendeesByIdUseCase()
    {
        _dbContext = new();
    }

    public ResponseAllAttendeesJson Execute(Guid eventId)
    {
        Event eventEntity = _dbContext.Events.Include(ev => ev.Attendees).ThenInclude(at => at.CheckIn).FirstOrDefault(ev => ev.Id == eventId) ??
            throw new NotFoundException($"Could not find event with id {eventId}.");

        var listResponse = eventEntity.Attendees
            .Select(attendee => new ResponseAttendeeJson()
            {
                Id = attendee.Id,
                Name = attendee.Name,
                Email = attendee.Email,
                CreatedAt = attendee.Created_At,
                CheckedInAt = attendee.CheckIn?.Created_at
            })
            .ToList();

        return new() { Attendees = listResponse };
    }
}
