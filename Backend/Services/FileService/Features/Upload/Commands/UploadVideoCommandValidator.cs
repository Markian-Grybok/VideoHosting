using FluentValidation;

namespace FileService.Features.Upload.Commands
{
    public class UploadVideoCommandValidator : AbstractValidator<UploadVideoCommand>
    {
        private static readonly string[] AllowedExtensions = new[] { ".mp4", ".mov", ".avi" };
        public UploadVideoCommandValidator()
        {
            RuleFor(x => x.File)
                .NotNull().WithMessage("File is required.")
                .Must(f => f != null && f.Length > 0).WithMessage("File must not be empty.")
                .Must(f => f != null && f.Length <= 524_288_000).WithMessage("File size must be <= 500MB.")
                .Must(f => f != null && AllowedExtensions.Contains(System.IO.Path.GetExtension(f.FileName).ToLower()))
                .WithMessage("File extension must be .mp4, .mov, or .avi.");
        }
    }
}