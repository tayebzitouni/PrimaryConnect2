using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace PrimaryConnect
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("NewMessage", user, message);
        }
    }
}
