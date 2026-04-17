namespace ContentService.API.Infrastructure.Http;

public record FileStatusResponse(
    Guid FileId,
    string Status,
    string? HlsManifestPath,
    DateTime CreatedAt,
    DateTime? ProcessedAt);

public record PlaybackUrlResponse(
    Guid FileId,
    string Url,
    int ExpiresIn);
