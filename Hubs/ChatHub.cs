using Proiect_MPA.Models;
using Microsoft.AspNetCore.SignalR;
namespace Proiect_MPA.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
