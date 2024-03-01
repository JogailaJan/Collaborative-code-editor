using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace CodeConnect.Hubs;

public class ChatHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }

    public async Task SendMessageToGroup(string user, string message, string room, bool join)
    {
         try
        {
            if (join)
            {
                await JoinRoom(room).ConfigureAwait(false);
                await Clients.Group(room).SendAsync("ReceiveMessage", user, " joined to " + room).ConfigureAwait(true);
            }
            else
            {
                await JoinRoom(room);
                await Clients.Group(room).SendAsync("ReceiveMessage", user, message).ConfigureAwait(true);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in SendMessageToGroup: {ex.Message}");
            throw; // Rethrow the exception to propagate it further if necessary
        }
    }

    public Task JoinRoom(string roomName)
    {
        return Groups.AddToGroupAsync(Context.ConnectionId, roomName);
    }

    public Task LeaveRoom(string roomName)
    {
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
    }
}