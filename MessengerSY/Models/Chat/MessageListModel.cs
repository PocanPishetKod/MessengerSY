using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerSY.Models.Chat
{
    public class MessageListModel
    {
        public IEnumerable<MessageModel> Messages { get; set; }
    }
}
