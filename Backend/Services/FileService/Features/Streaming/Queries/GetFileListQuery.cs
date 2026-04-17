using MediatR;
using FileService.Features.Streaming.Dtos;

namespace FileService.Features.Streaming.Queries
{
    public record GetFileListQuery(int Page, int PageSize) : IRequest<FileListDto>;
}
