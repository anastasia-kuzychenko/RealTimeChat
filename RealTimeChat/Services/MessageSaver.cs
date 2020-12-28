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
        private readonly MessageConfig messageConfig;
        private readonly IRepository<Message> messageRepo;
        private readonly IRepository<UserMessages> userMessagesRepo;

        private static Dictionary<string, List<Message>> UserMessages = new Dictionary<string, List<Message>>();
        private static List<Message> Messages = new List<Message>();

        static bool notLoaded = true;

        public MessageSaver(IOptions<MessageConfig> config, IRepository<Message> messageRepo, IRepository<UserMessages> userMessagesRepo)
        {
            messageConfig = config.Value;
            this.messageRepo = messageRepo;
            this.userMessagesRepo = userMessagesRepo;
            if (notLoaded)
            {
                LoadFromDb().Wait();
                notLoaded = false;
            }
        }

        private async Task LoadFromDb()
        {
            Messages = await messageRepo.Get();
            var users = await userMessagesRepo.Get();
            UserMessages = users.ToDictionary(um=> um.Nickname,
                um => um.Messages.Select(x=>new Message { Sender = um.Nickname, Text = x }).ToList());
        }

        public async Task SaveMessages()
        {
            var messages = await messageRepo.Get();
            if(messages.Count == 0)
                await messageRepo.Create(Messages);
            else
            {
                for (int i = 0; i < messages.Count; i++)
                {
                    messages[i].Sender = Messages[i].Sender;
                    messages[i].Text = Messages[i].Text;
                }

                var messagesToCreate = Messages.Skip(messages.Count);
                
                await messageRepo.Create(messagesToCreate);
                await messageRepo.Update(messages);
            }
        }

        public async Task SaveMessages(string nikname)
        {
            var user = await userMessagesRepo.FindFirstOrDefault(x => x.Nickname == nikname);
            var messages = UserMessages[nikname].Select(x => x.Text).ToList();
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
            if (Messages.Count == messageConfig.MaxMessageSize)
                Messages.RemoveAt(0);
            Messages.Add(message);

            if (UserMessages.ContainsKey(message.Sender))
            {
                var messages = UserMessages[message.Sender];
                if (messages.Count == messageConfig.MaxUserMessageSize)
                    messages.RemoveAt(0);
                messages.Add(message);
            }
            else
                UserMessages.Add(message.Sender, new List<Message> { message });
        }

        public IEnumerable<Message> GetMessages()
        {
            return Messages.AsReadOnly();
        }

        public IEnumerable<Message> GetMessages(string nickname)
        {
            if(UserMessages.ContainsKey(nickname))
                return UserMessages[nickname].AsReadOnly();
            return Enumerable.Empty<Message>();
        }
    }
}
