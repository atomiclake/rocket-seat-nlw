using PassIn.Communication.Requests;
using PassIn.Communication.Responses;
using PassIn.Exceptions;
using PassIn.Infrastructure;
using PassIn.Infrastructure.Entities;

using System.Net.Mail;

namespace PassIn.Application.UseCases.Events.RegisterAttendee;

public class RegisterAttendeeUseCase
{
    private readonly PassInDbContext _dbContext;

    public RegisterAttendeeUseCase()
    {
        _dbContext = new();
    }

    public ResponseResourceRegisteredJson Execute(Guid eventId, RequestRegisterEventJson request)
    {
        Validate(request, eventId);

        Attendee attendee = new()
        {
            Name = request.Name,
            Email = request.Email,
            Created_At = DateTime.UtcNow,
            Event_Id = eventId
        };

        _dbContext.Attendees.Add(attendee);
        _dbContext.SaveChanges();

        return new() { Id = attendee.Id };
    }

    private void Validate(RequestRegisterEventJson request, Guid eventId)
    {
        Event eventEntity = _dbContext.Events.Find(eventId) ?? throw new NotFoundException($"Could not find event with id '{eventId}'.");

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ErrorOnValidationException("The name field is invalid. It must not be empty or contain only whitespaces.");
        }

        if (!IsEmailValid(request.Email))
        {
            throw new ErrorOnValidationException("The email field is invalid. It must contain a properly formatted email address.");
        }

        int attendeeCount = _dbContext.Attendees.Count(attendee => attendee.Event_Id == eventId);

        if (attendeeCount == eventEntity.MaximumAttendees)
        {
            throw new RequestConflictException($"This event has reached its' maximum capacity for {attendeeCount} attendee(s).");
        }

        bool attendeeIsAlreadyRegistered = _dbContext.Attendees
            .Any(attendee => attendee.Email.Equals(request.Email) && attendee.Event_Id == eventId);

        if (attendeeIsAlreadyRegistered)
        {
            throw new RequestConflictException($"The attendee '{request.Name}' is already registered to this event.");
        }
    }

    private bool IsEmailValid(string email)
    {
        return MailAddress.TryCreate(email, out _);
    }
}
