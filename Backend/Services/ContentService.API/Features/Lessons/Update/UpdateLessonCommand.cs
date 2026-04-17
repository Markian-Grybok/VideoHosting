using FluentResults;
using MediatR;

namespace ContentService.API.Features.Lessons.Update;

public record UpdateLessonCommand(
    Guid Id, 
    string Title, 
    string Description, 
    int Order, 
    Guid? VideoFileId = null) : IRequest<Result<UpdateLessonResponse>>;
