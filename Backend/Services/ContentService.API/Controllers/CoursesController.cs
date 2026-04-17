using ContentService.API.Features.Courses.Create;
using ContentService.API.Features.Courses.Delete;
using ContentService.API.Features.Courses.GetAll;
using ContentService.API.Features.Courses.GetById;
using ContentService.API.Features.Courses.Update;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ContentService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CoursesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<CourseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllCoursesQuery(), cancellationToken);

        if (result.IsFailed)
            return BadRequest(result.Errors.Select(e => e.Message));

        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CourseDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCourseByIdQuery(id), cancellationToken);

        if (result.IsFailed)
        {
            var errorMessage = result.Errors.First().Message;
            if (errorMessage.Contains("not found"))
                return NotFound(result.Errors.Select(e => e.Message));
            return BadRequest(result.Errors.Select(e => e.Message));
        }

        return Ok(result.Value);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateCourseResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateCourseRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateCourseCommand(request.Title, request.Description);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailed)
            return BadRequest(result.Errors.Select(e => e.Message));

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value.Id },
            result.Value);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UpdateCourseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateCourseRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCourseCommand(id, request.Title, request.Description);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            var errorMessage = result.Errors.First().Message;
            if (errorMessage.Contains("not found"))
                return NotFound(result.Errors.Select(e => e.Message));
            return BadRequest(result.Errors.Select(e => e.Message));
        }

        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteCourseCommand(id), cancellationToken);

        if (result.IsFailed)
        {
            var errorMessage = result.Errors.First().Message;
            if (errorMessage.Contains("not found"))
                return NotFound(result.Errors.Select(e => e.Message));
            return BadRequest(result.Errors.Select(e => e.Message));
        }

        return NoContent();
    }
}
