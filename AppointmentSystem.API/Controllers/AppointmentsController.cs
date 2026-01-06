using AppointmentSystem.Application.Features.Appointments.Commands.CreateAppointment;
using AppointmentSystem.Application.Features.Appointments.Queries.GetAppointmentsByPatient;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AppointmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("patient/{patientId}")]
    public async Task<IActionResult> GetAppointmentsByPatient(int patientId)
    {
        var query = new GetAppointmentsByPatientQuery { PatientId = patientId };
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
