namespace FileService.Features.Processing
{
    public class FfmpegOptions
    {
        public string BinaryPath { get; set; } = "ffmpeg";
        public int SegmentDuration { get; set; } = 6;
    }
}