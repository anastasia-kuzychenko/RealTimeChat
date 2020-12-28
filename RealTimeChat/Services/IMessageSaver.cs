using RealTimeChat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealTimeChat.Services
{
    public interface IMessageSaver
    {
        void AddMessage(Message message);
        IEnumerable<Message> GetMessages();
        IEnumerable<Message> GetMessages(string nickname);
        Task SaveMessages();
        Task SaveMessages(string nickname);
    }
}
