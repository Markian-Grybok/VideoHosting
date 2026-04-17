using MediatR;
using Microsoft.EntityFrameworkCore;
using FileService.Features.Streaming.Dtos;
using FileService.Infrastructure.Persistence;

namespace FileService.Features.Streaming.Queries
{
    public class GetFileListQueryHandler : IRequestHandler<GetFileListQuery, FileListDto>
    {
        private readonly FileServiceDbContext _dbContext;

        public GetFileListQueryHandler(FileServiceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<FileListDto> Handle(GetFileListQuery request, CancellationToken cancellationToken)
        {
            var total = await _dbContext.VideoFiles.CountAsync(cancellationToken);

            var items = await _dbContext.VideoFiles
                .OrderByDescending(f => f.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(f => new FileStatusDto(
                    f.Id,
                    f.OriginalFileName,
                    f.Status.ToString(),
                    f.HlsManifestPath,
                    f.CreatedAt,
                    f.ProcessedAt,
                    f.ErrorMessage))
                .ToListAsync(cancellationToken);

            return new FileListDto(items, total, request.Page, request.PageSize);
        }
    }
}
