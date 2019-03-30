using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerSY.Models.Chat
{
    public class ReceiveChatModel
    {
        public int ChatId { get; set; }
        public string Title { get; set; }
        public bool IsGroup { get; set; }
        public MessageModel Message { get; set; }
        public IEnumerable<UserProfileModel> Participants { get; set; }
    }
}
