using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerSY.Models.Chat
{
    public class ChatListModel
    {
        public IEnumerable<ChatModel> Chats { get; set; }
    }
}
