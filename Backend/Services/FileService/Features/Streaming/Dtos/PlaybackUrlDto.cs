namespace FileService.Features.Streaming.Dtos
{
    public record PlaybackUrlDto(Guid FileId, string Url, int ExpiresIn);
}
