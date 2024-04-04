using Microsoft.AspNetCore.Mvc;

using PassIn.Application.UseCases.Events.GetById;
using PassIn.Application.UseCases.Events.Register;
using PassIn.Communication.Requests;
using PassIn.Communication.Responses;

namespace PassIn.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EventsController : ControllerBase
{
    // O endpoint "Register" espera que os dados da requisição sejam enviados através do corpo da requisição.
    [HttpPost]
    [ProducesResponseType(typeof(ResponseResourceRegisteredJson), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    public IActionResult Register([FromBody] RequestEventJson request)
    {
        // Aqui criamos uma instância da classe que representa a regra de negócio sendo utilizada atualmente (registrar eventos).
        var useCase = new RegisterEventUseCase();

        // E passamos a requisição para ser processada na camada de Aplicação.
        ResponseResourceRegisteredJson response = useCase.Execute(request);

        // O método "Created" corresponde ao código HTTP 201, então temos como retornar 200 OK ou outros códigos mais apropriados.
        return Created(string.Empty, response);
    }

    [HttpGet]
    [Route("{id}")]
    [ProducesResponseType(typeof(ResponseEventJson), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public IActionResult GetById([FromRoute] Guid id)
    {
        var useCase = new GetEventByIdUseCase();

        ResponseEventJson response = useCase.Execute(id);

        return Ok(response);
    }
}
