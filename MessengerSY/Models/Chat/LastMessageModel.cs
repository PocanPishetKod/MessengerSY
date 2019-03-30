using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerSY.Models.Chat
{
    public class LastMessageModel
    {
        public string TextContent { get; set; }
        public UserProfileModel Sender { get; set; }
    }
}
