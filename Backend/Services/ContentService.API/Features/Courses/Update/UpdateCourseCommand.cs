using FluentResults;
using MediatR;

namespace ContentService.API.Features.Courses.Update;

public record UpdateCourseCommand(Guid Id, string Title, string Description)
    : IRequest<Result<UpdateCourseResponse>>;
