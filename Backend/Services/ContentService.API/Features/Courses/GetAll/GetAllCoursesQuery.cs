using FluentResults;
using MediatR;

namespace ContentService.API.Features.Courses.GetAll;

public record GetAllCoursesQuery : IRequest<Result<List<CourseDto>>>;
