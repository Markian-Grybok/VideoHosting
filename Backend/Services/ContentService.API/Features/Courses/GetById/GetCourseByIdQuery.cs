using FluentResults;
using MediatR;

namespace ContentService.API.Features.Courses.GetById;

public record GetCourseByIdQuery(Guid Id) : IRequest<Result<CourseDetailsDto>>;
