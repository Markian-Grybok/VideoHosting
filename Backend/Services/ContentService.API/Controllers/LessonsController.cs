using ContentService.API.Features.Lessons.Create;
using ContentService.API.Features.Lessons.Delete;
using ContentService.API.Features.Lessons.GetAll;
using ContentService.API.Features.Lessons.GetById;
using ContentService.API.Features.Lessons.Update;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ContentService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LessonsController : ControllerBase
{
    private readonly IMediator _mediator;

    public LessonsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<LessonDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? courseId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllLessonsQuery(courseId), cancellationToken);

        if (result.IsFailed)
            return BadRequest(result.Errors.Select(e => e.Message));

        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(LessonDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetLessonByIdQuery(id), cancellationToken);

        if (result.IsFailed)
            return NotFound(result.Errors.Select(e => e.Message));

        return Ok(result.Value);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateLessonResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateLessonRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateLessonCommand(
            request.Title, 
            request.Description, 
            request.CourseId, 
            request.Order, 
            request.VideoFileId);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailed)
            return BadRequest(result.Errors.Select(e => e.Message));

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value.Id },
            result.Value);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UpdateLessonResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateLessonRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateLessonCommand(
            id, 
            request.Title, 
            request.Description, 
            request.Order, 
            request.VideoFileId);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailed)
            return NotFound(result.Errors.Select(e => e.Message));

        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteLessonCommand(id), cancellationToken);

        if (result.IsFailed)
            return NotFound(result.Errors.Select(e => e.Message));

        return NoContent();
    }
}