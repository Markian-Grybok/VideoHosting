using MediatR;
using FileService.Features.Streaming.Dtos;

namespace FileService.Features.Streaming.Queries
{
    public record GetPlaybackUrlQuery(Guid FileId) : IRequest<PlaybackUrlDto>;
}
