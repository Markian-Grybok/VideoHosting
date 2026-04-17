using ContentService.API.Features.Lessons.Create;
using FluentValidation;

namespace ContentService.API.Features.Lessons.Create;

public class CreateLessonCommandValidator : AbstractValidator<CreateLessonCommand>
{
    public CreateLessonCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters");

        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("CourseId is required");

        RuleFor(x => x.Order)
            .GreaterThan(0).WithMessage("Order must be greater than 0");
    }
}
