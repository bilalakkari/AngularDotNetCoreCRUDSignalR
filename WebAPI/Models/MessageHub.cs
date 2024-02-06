using Microsoft.AspNetCore.SignalR;
using SignalRDemo.Hub;

namespace WebAPI.Models
{
    public class MessageHub : Hub<IMessageHubClient>
    {
        public async Task SendOffersToUser(USER user)
        {
            await Clients.All.SendOffersToUser(user);
        }
        public async Task UpdateOffersToUser(USER user)
        {
            await Clients.All.UpdateOffersToUser(user);
        }

        public async Task RemoveOffersToUser(USER user)
        {
            await Clients.All.RemoveOffersToUser(user);
        }
    }
}