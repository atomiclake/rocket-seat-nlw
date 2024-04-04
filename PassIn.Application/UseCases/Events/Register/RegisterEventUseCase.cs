using PassIn.Communication.Requests;
using PassIn.Communication.Responses;
using PassIn.Exceptions;
using PassIn.Infrastructure;
using PassIn.Infrastructure.Entities;

namespace PassIn.Application.UseCases.Events.Register;

public class RegisterEventUseCase
{
    public ResponseResourceRegisteredJson Execute(RequestEventJson request)
    {
        Validate(request);

        var dbContext = new PassInDbContext();

        var entity = new Event()
        {
            Title = request.Title,
            Details = request.Details,
            MaximumAttendees = request.MaximumAttendees,
            Slug = request.Title.ToLower().Replace(" ", "-")
        };

        dbContext.Events.Add(entity);
        dbContext.SaveChanges();

        return new() { Id = entity.Id };
    }

    // Aqui realizamos a validação das informações recebidas pela requisição.
    // Caso algum parâmetro esteja fora do que é esperado, lançamos uma exceção.
    private void Validate(RequestEventJson request)
    {
        if (request.MaximumAttendees <= 0)
        {
            throw new ErrorOnValidationException("The maximum number of attendees is invalid. It must be greater than 0.");
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ErrorOnValidationException("The title field is invalid. It must not be empty or contain only whitespaces.");
        }

        if (string.IsNullOrWhiteSpace(request.Details))
        {
            throw new ErrorOnValidationException("The details field is invalid. It must not be empty or contain only whitespaces.");
        }
    }
}
