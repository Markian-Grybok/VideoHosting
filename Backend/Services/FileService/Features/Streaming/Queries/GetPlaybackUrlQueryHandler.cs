using MediatR;
using FileService.Common.Entities;
using FileService.Infrastructure.Persistence;
using FileService.Infrastructure.Storage;
using FileService.Features.Streaming.Dtos;

namespace FileService.Features.Streaming.Queries
{
    public class GetPlaybackUrlQueryHandler : IRequestHandler<GetPlaybackUrlQuery, PlaybackUrlDto>
    {
        private readonly FileServiceDbContext _dbContext;
        private readonly IStorageService _storageService;
        private const int PresignedUrlExpirySeconds = 3600;

        public GetPlaybackUrlQueryHandler(FileServiceDbContext dbContext, IStorageService storageService)
        {
            _dbContext = dbContext;
            _storageService = storageService;
        }

        public async Task<PlaybackUrlDto> Handle(GetPlaybackUrlQuery request, CancellationToken cancellationToken)
        {
            var video = await _dbContext.VideoFiles.FindAsync(new object[] { request.FileId }, cancellationToken);

            if (video == null)
                throw new KeyNotFoundException($"Video file not found: {request.FileId}");

            if (video.Status != VideoFileStatus.Ready)
                throw new InvalidOperationException("Video is not ready for playback");

            var url = await _storageService.GetPresignedUrlAsync(video.HlsManifestPath, PresignedUrlExpirySeconds, cancellationToken);

            return new PlaybackUrlDto(request.FileId, url, PresignedUrlExpirySeconds);
        }
    }
}
