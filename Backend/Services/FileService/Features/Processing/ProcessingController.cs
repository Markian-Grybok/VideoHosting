using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using FileService.Features.Processing.Queries;

namespace FileService.Features.Processing
{
    [ApiController]
    [Route("api/files")]
    public class ProcessingController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ProcessingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{fileId}/processing-status")]
        public async Task<IActionResult> GetProcessingStatus(Guid fileId, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetProcessingStatusQuery(fileId), ct);
            if (result == null) return NotFound();
            return Ok(result);
        }
    }
}
