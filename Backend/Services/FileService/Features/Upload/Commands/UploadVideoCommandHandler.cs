using FileService.Common.Entities;
using FileService.Infrastructure.Messaging;
using FileService.Infrastructure.Persistence;
using FileService.Infrastructure.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MediatR;
using System.IO;

namespace FileService.Features.Upload.Commands
{
    public class UploadVideoCommandHandler : IRequestHandler<UploadVideoCommand, UploadVideoResult>
    {
        private readonly IStorageService _storageService;
        private readonly FileServiceDbContext _dbContext;
        private readonly IMessagePublisher _publisher;
        private readonly IOptions<RabbitMqOptions> _rabbitOptions;
        private readonly ILogger<UploadVideoCommandHandler> _logger;

        public UploadVideoCommandHandler(
            IStorageService storageService,
            FileServiceDbContext dbContext,
            IMessagePublisher publisher,
            IOptions<RabbitMqOptions> rabbitOptions,
            ILogger<UploadVideoCommandHandler> logger)
        {
            _storageService = storageService;
            _dbContext = dbContext;
            _publisher = publisher;
            _rabbitOptions = rabbitOptions;
            _logger = logger;
        }

        public async Task<UploadVideoResult> Handle(UploadVideoCommand command, CancellationToken cancellationToken)
        {
            var file = command.File;
            var objectName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            using (var stream = file.OpenReadStream())
            {
                await _storageService.UploadFileAsync(stream, objectName, file.ContentType, cancellationToken);
            }

            var entity = new VideoFile
            {
                OriginalFileName = file.FileName,
                StoragePath = objectName,
                Status = VideoFileStatus.Pending
            };
            _dbContext.VideoFiles.Add(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var evt = new VideoUploadedEvent
            {
                FileId = entity.Id,
                OriginalFileName = entity.OriginalFileName,
                StoragePath = entity.StoragePath,
                OccurredAt = DateTime.UtcNow
            };
            await _publisher.PublishAsync(evt, _rabbitOptions.Value.QueueName, cancellationToken);

            return new UploadVideoResult(entity.Id, entity.OriginalFileName, entity.Status.ToString());
        }
    }
}