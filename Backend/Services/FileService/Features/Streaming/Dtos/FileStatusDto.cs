namespace FileService.Features.Streaming.Dtos
{
    public record FileStatusDto(
        Guid FileId,
        string OriginalFileName,
        string Status,
        string? HlsManifestPath,
        DateTime CreatedAt,
        DateTime? ProcessedAt,
        string? ErrorMessage);
}
