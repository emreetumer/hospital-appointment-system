using AppointmentSystem.Application.Features.Departments.Queries.GetAllDepartments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DepartmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DepartmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllDepartments()
    {
        var query = new GetAllDepartmentsQuery();
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
