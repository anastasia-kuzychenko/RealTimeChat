using RealTimeChat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealTimeChat.Hubs
{
    public interface IMessageClient
    {
        Task Send(Message message);
        Task OnNewUserConnected(string nickname);
        Task OnDisconnected(string nickname);
        Task GetHistory(IEnumerable<Message> messages);
    }
}
