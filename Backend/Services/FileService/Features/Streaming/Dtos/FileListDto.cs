namespace FileService.Features.Streaming.Dtos
{
    public record FileListDto(List<FileStatusDto> Items, int TotalCount, int Page, int PageSize);
}
