namespace FileService.Infrastructure.Messaging
{
    public class VideoUploadedEvent
    {
        public Guid FileId { get; set; }
        public string StoragePath { get; set; } = string.Empty;
        public string OriginalFileName { get; set; } = string.Empty;
        public DateTime OccurredAt { get; set; }
    }
}
