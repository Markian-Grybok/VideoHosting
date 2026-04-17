namespace ContentService.API.Infrastructure.Http;

public class FileServiceOptions
{
    public string BaseUrl { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 30;
}
