using FluentResults;
using MediatR;

namespace ContentService.API.Features.Courses.Delete;

public record DeleteCourseCommand(Guid Id) : IRequest<Result>;
