using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealTimeChat.Models
{
    public sealed class Message
    {
        public int Id { get; set; }
        public string Sender { get; set; }
        public string Text { get; set; }
    }
}
