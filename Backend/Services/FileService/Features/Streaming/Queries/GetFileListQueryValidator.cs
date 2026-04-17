using FluentValidation;
using FileService.Features.Streaming.Queries;

namespace FileService.Features.Streaming.Queries
{
    public class GetFileListQueryValidator : AbstractValidator<GetFileListQuery>
    {
        public GetFileListQueryValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page must be greater than or equal to 1.");

            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(1)
                .LessThanOrEqualTo(100)
                .WithMessage("PageSize must be between 1 and 100.");
        }
    }
}
