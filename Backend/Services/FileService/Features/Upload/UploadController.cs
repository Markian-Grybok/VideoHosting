using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileService.Features.Upload
{
    [ApiController]
    [Route("api/files")]
    public class UploadController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UploadController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("upload")]
        [RequestSizeLimit(524_288_000)]
        [RequestFormLimits(MultipartBodyLengthLimit = 524_288_000)]
        public async Task<IActionResult> Upload(
            IFormFile file,
            CancellationToken ct)
        {
            var result = await _mediator.Send(new Commands.UploadVideoCommand(file), ct);
            return Accepted(new { result.FileId, result.OriginalFileName, result.Status });
        }
    }
}
