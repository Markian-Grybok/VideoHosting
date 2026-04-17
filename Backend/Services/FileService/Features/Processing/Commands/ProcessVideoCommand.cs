using System;
using FileService.Common.Entities;
using MediatR;

namespace FileService.Features.Processing.Commands
{
    public record ProcessVideoCommand(Guid FileId, string StoragePath) : IRequest<ProcessVideoResult>;
    public record ProcessVideoResult(Guid FileId, VideoFileStatus Status);
}