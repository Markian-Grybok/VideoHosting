using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace FileService.Features.Processing.Hubs
{
    /// <summary>
    /// Client event: "ProcessingUpdate"
    /// Payload: { fileId: string, status: "Processing" | "Ready" | "Failed", progressPercent: number }
    /// </summary>
    [Authorize(AuthenticationSchemes = "Bearer")] // remove this line if no auth
    public class ProcessingHub : Hub
    {
        public async Task SubscribeToFile(string fileId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"file-{fileId}");
        }

        public async Task UnsubscribeFromFile(string fileId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"file-{fileId}");
        }
    }
}
