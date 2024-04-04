using Microsoft.EntityFrameworkCore;

using PassIn.Communication.Responses;
using PassIn.Exceptions;
using PassIn.Infrastructure;
using PassIn.Infrastructure.Entities;

namespace PassIn.Application.UseCases.Events.GetById;

public class GetEventByIdUseCase
{
    private readonly PassInDbContext _dbContext;

    public GetEventByIdUseCase()
    {
        _dbContext = new PassInDbContext();
    }

    public ResponseEventJson Execute(Guid id)
    {
        Event entity = _dbContext.Events.Include(ev => ev.Attendees).FirstOrDefault(ev => ev.Id == id)
            ?? throw new NotFoundException($"Could not find event with id '{id}'.");

        return new()
        {
            Id = entity.Id,
            Title = entity.Title,
            Details = entity.Details,
            MaximumAttendees = entity.MaximumAttendees,
            AttendeesAmount = entity.Attendees.Count
        };
    }
}
