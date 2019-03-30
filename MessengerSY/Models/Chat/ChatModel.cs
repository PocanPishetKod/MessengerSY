using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerSY.Models.Chat
{
    public class ChatModel
    {
        public int ChatId { get; set; }
        public string Title { get; set; }
        public bool IsGroup { get; set; }
        public LastMessageModel LastMessage { get; set; }
        public IEnumerable<UserProfileModel> Participants { get; set; }
    }
}
