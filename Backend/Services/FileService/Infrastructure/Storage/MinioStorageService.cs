using Minio;
using Minio.DataModel.Args;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using FileService.Common.Exceptions;

namespace FileService.Infrastructure.Storage
{
    public class MinioStorageService : IStorageService
    {
        private readonly IMinioClient _minioClient;
        private readonly MinioOptions _options;
        private readonly ILogger<MinioStorageService> _logger;

        public MinioStorageService(IOptions<MinioOptions> options, ILogger<MinioStorageService> logger)
        {
            _options = options.Value;
            _logger = logger;

            _minioClient = new MinioClient()
                .WithEndpoint(_options.Endpoint)
                .WithCredentials(_options.AccessKey, _options.SecretKey)
                .WithSSL(_options.UseSSL)
                .Build();

            EnsureBucketExistsAsync().GetAwaiter().GetResult();
        }

        public async Task<string> UploadFileAsync(Stream stream, string objectName, string contentType, CancellationToken ct)
        {
            try
            {
                // Save stream to temporary file and upload using WithFileName
                var tempPath = Path.GetTempFileName();
                try
                {
                    using (var fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write))
                    {
                        await stream.CopyToAsync(fileStream, ct);
                    }

                    var putObjectArgs = new PutObjectArgs()
                        .WithBucket(_options.BucketName)
                        .WithObject(objectName)
                        .WithFileName(tempPath)
                        .WithContentType(contentType);

                    await _minioClient.PutObjectAsync(putObjectArgs, ct);
                    _logger.LogInformation($"File uploaded successfully: {objectName}");
                    return objectName;
                }
                finally
                {
                    if (File.Exists(tempPath))
                    {
                        File.Delete(tempPath);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error uploading file: {objectName}");
                throw new StorageException($"Failed to upload file: {objectName}", ex);
            }
        }

        public async Task DownloadFileAsync(string objectName, string destinationPath, CancellationToken ct)
        {
            try
            {
                var directory = Path.GetDirectoryName(destinationPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write))
                {
                    var getObjectArgs = new GetObjectArgs()
                        .WithBucket(_options.BucketName)
                        .WithObject(objectName)
                        .WithCallbackStream((stream) =>
                        {
                            stream.CopyTo(fileStream);
                        });

                    await _minioClient.GetObjectAsync(getObjectArgs, ct);
                }

                _logger.LogInformation($"File downloaded successfully: {objectName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error downloading file: {objectName}");
                throw new StorageException($"Failed to download file: {objectName}", ex);
            }
        }

        public async Task UploadDirectoryAsync(string localDirectory, string storagePrefix, CancellationToken ct)
        {
            try
            {
                if (!Directory.Exists(localDirectory))
                {
                    throw new StorageException($"Directory does not exist: {localDirectory}");
                }

                var files = Directory.GetFiles(localDirectory, "*", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    var relativePath = Path.GetRelativePath(localDirectory, file);
                    var objectName = $"{storagePrefix}/{relativePath.Replace("\\", "/")}";



                    using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
                    {
                        var contentType = GetContentType(file);
                        await UploadFileAsync(fileStream, objectName, contentType, ct);
                    }
                }

                _logger.LogInformation($"Directory uploaded successfully: {localDirectory}");
            }
            catch (StorageException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error uploading directory: {localDirectory}");
                throw new StorageException($"Failed to upload directory: {localDirectory}", ex);
            }
        }

        public async Task<string> GetPresignedUrlAsync(string objectName, int expirySeconds, CancellationToken ct)
        {
            try
            {
                var presignedGetObjectArgs = new PresignedGetObjectArgs()
                    .WithBucket(_options.BucketName)
                    .WithObject(objectName)
                    .WithExpiry(expirySeconds);

                var url = await _minioClient.PresignedGetObjectAsync(presignedGetObjectArgs);
                _logger.LogInformation($"Presigned URL generated for: {objectName}");
                return url;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating presigned URL: {objectName}");
                throw new StorageException($"Failed to generate presigned URL: {objectName}", ex);
            }
        }

        private async Task EnsureBucketExistsAsync()
        {
            try
            {
                var beArgs = new BucketExistsArgs()
                    .WithBucket(_options.BucketName);

                bool found = await _minioClient.BucketExistsAsync(beArgs);
                if (!found)
                {
                    var mbArgs = new MakeBucketArgs()
                        .WithBucket(_options.BucketName);

                    await _minioClient.MakeBucketAsync(mbArgs);
                    _logger.LogInformation($"Bucket created: {_options.BucketName}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error ensuring bucket exists: {_options.BucketName}");
                throw new StorageException($"Failed to ensure bucket exists: {_options.BucketName}", ex);
            }
        }

        private string GetContentType(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return extension switch
            {
                ".mp4" => "video/mp4",
                ".m3u8" => "application/vnd.apple.mpegurl",
                ".ts" => "video/mp2t",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                ".json" => "application/json",
                ".txt" => "text/plain",
                _ => "application/octet-stream"
            };
        }
    }
}
