using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace CodeConnect.Hubs
{
    public class CodeHub : Hub
    {
        public async Task SendCodeUpdate(string code)
        {
            // Broadcast code to all clients except the sender
            await Clients.Others.SendAsync("ReceiveCodeUpdate", code);
        }
    }

}