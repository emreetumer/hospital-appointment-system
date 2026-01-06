using AppointmentSystem.Application.Features.Doctors.Queries.GetAllDoctors;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DoctorsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DoctorsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllDoctors()
    {
        var query = new GetAllDoctorsQuery();
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
