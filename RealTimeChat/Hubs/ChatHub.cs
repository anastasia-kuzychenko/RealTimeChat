using Microsoft.AspNetCore.SignalR;
using RealTimeChat.Data;
using RealTimeChat.Services;
using RealTimeChat.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RealTimeChat.Hubs
{
    public class MessageHub : Hub<IMessageClient>
    {
        private readonly IMessageSaver messageSaver;
        private readonly IRepository<User> userRepo;
        private readonly Semaphore sem = new Semaphore(1, 1);

        private int Count { get; set; } = 0;

        public MessageHub(IMessageSaver messageSaver, IRepository<User> userRepo)
        {
            this.messageSaver = messageSaver;
            this.userRepo = userRepo;
        }

        public async Task SendToOthers(string message)
        {
            var nickname = Context.Items["Nickname"] as string;
            if (nickname != null)
            {
                var messageForClient = new Message { Sender = nickname, Text = message };

                // to prevent parallel access to message list
                sem.WaitOne();
                messageSaver.AddMessage(messageForClient);
                sem.Release();

                await Clients.Others.Send(messageForClient);
            }
        }

        public async Task Connect(string nickname)
        {
            var user = await userRepo.FindFirstOrDefault(x => x.Nickname == nickname);
            if (user == null)
            {
                if(await userRepo.Create(new User { Nickname = nickname }))
                {
                    Context.Items.Add("Nickname", nickname);

                    var messageHistory = messageSaver.GetMessages();
                    Count++;
                    await Clients.Caller.GetHistory(messageHistory);
                    await Clients.All.OnNewUserConnected(nickname);
                }
            }
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {

            var nickname = Context.Items["Nickname"] as string;
            Clients.All.OnDisconnected(nickname);
            messageSaver.SaveMessages(nickname);
            if (Count == 1)
                messageSaver.SaveMessages();
            Count--;
            return base.OnDisconnectedAsync(exception);
        }
    }
}
