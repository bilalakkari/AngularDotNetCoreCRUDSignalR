using WebAPI.Models;

namespace SignalRDemo.Hub
{
    public interface IMessageHubClient
    {
        Task SendOffersToUser(USER user);
        Task UpdateOffersToUser(USER user);
        Task RemoveOffersToUser(USER user);
    }
}