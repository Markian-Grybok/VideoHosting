using FluentResults;
using MediatR;

namespace ContentService.API.Features.Courses.Create;

public record CreateCourseCommand(string Title, string Description)
    : IRequest<Result<CreateCourseResponse>>;
