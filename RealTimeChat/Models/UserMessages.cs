using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealTimeChat.Models
{
    public class UserMessages
    {
        public int Id { get; set; }
        public string Nickname { get; set; }
        public List<string> Messages { get; set; }
    }
}
