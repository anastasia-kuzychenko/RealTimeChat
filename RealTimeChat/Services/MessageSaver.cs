using Microsoft.Extensions.Options;
using RealTimeChat.Config;
using RealTimeChat.Data;
using RealTimeChat.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealTimeChat.Services
{
    public class MessageSaver : IMessageSaver
    {
        private Dictionary<string, List<Message>> userMessages;
        private List<Message> messages;

        private readonly MessageConfig messageConfig;
        private readonly IRepository<Message> messageRepo;
        private readonly IRepository<UserMessages> userMessagesRepo;

        public MessageSaver(IOptions<MessageConfig> config, IRepository<Message> messageRepo, IRepository<UserMessages> userMessagesRepo)
        {
            messageConfig = config.Value;
            this.messageRepo = messageRepo;
            this.userMessagesRepo = userMessagesRepo;
            userMessages = new Dictionary<string, List<Message>>();
            messages = new List<Message>(messageConfig.MaxMessageSize);

            LoadFromDb().Wait();
        }

        private async Task LoadFromDb()
        {
            messages = await messageRepo.Get();
            var users = await userMessagesRepo.Get();
            userMessages = users.ToDictionary(um=> um.Nickname,
                um => um.Messages.Select(x=>new Message { Sender = um.Nickname, Text = x }).ToList());
        }

        public async Task SaveMessages()
        {
            for (int i = 1; i < messages.Count; i++)
            {
                messages[i].Id = i;
            }
            await messageRepo.Update(messages);
        }

        public async Task SaveMessages(string nikname)
        {
            var user = await userMessagesRepo.FindFirstOrDefault(x => x.Nickname == nikname);
            var messages = userMessages[nikname].Select(x => x.Text).ToList();
            if (user == null)
                await userMessagesRepo.Create(new UserMessages { Nickname = nikname, Messages = messages });
            else
            {
                user.Messages = messages;
                await userMessagesRepo.Update(user);
            }
        }

        public void AddMessage(Message message)
        {
            if (messages.Count == messageConfig.MaxMessageSize)
                messages.RemoveAt(0);
            messages.Add(message);

            if (userMessages.ContainsKey(message.Sender))
            {
                var messages = userMessages[message.Sender];
                if (messages.Count == messageConfig.MaxUserMessageSize)
                    messages.RemoveAt(0);
                messages.Add(message);
            }
            else
                userMessages.Add(message.Sender, new List<Message> { message });
        }

        public IEnumerable<Message> GetMessages()
        {
            return messages.AsReadOnly();
        }

        public IEnumerable<Message> GetMessages(string nickname)
        {
            if(userMessages.ContainsKey(nickname))
                return userMessages[nickname].AsReadOnly();
            return Enumerable.Empty<Message>();
        }
    }
}
