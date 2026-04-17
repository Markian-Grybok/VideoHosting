using System;
using MediatR;

namespace FileService.Features.Processing.Queries
{
    public record GetProcessingStatusQuery(Guid FileId) : IRequest<ProcessingStatusResult?>;
    public record ProcessingStatusResult(Guid FileId, string Status, int? Progress, string? Error);
}
