using MediatR;
using Microsoft.EntityFrameworkCore;
using FileService.Features.Streaming.Dtos;
using FileService.Infrastructure.Persistence;

namespace FileService.Features.Streaming.Queries
{
    public class GetFileStatusQueryHandler : IRequestHandler<GetFileStatusQuery, FileStatusDto?>
    {
        private readonly FileServiceDbContext _dbContext;

        public GetFileStatusQueryHandler(FileServiceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<FileStatusDto?> Handle(GetFileStatusQuery request, CancellationToken cancellationToken)
        {
            var video = await _dbContext.VideoFiles.FindAsync(new object[] { request.FileId }, cancellationToken);

            if (video == null)
                return null;

            return new FileStatusDto(
                video.Id,
                video.OriginalFileName,
                video.Status.ToString(),
                video.HlsManifestPath,
                video.CreatedAt,
                video.ProcessedAt,
                video.ErrorMessage);
        }
    }
}
