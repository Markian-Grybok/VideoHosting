using MediatR;
using FileService.Features.Streaming.Dtos;

namespace FileService.Features.Streaming.Queries
{
    public record GetFileStatusQuery(Guid FileId) : IRequest<FileStatusDto?>;
}
