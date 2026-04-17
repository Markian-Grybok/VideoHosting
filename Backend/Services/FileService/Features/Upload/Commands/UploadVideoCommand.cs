using MediatR;
using Microsoft.AspNetCore.Http;

namespace FileService.Features.Upload.Commands
{
    public record UploadVideoCommand(IFormFile File) : IRequest<UploadVideoResult>;

    public record UploadVideoResult(Guid FileId, string OriginalFileName, string Status);
}