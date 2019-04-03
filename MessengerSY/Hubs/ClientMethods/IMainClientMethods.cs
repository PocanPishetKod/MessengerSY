using MessengerSY.Models.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerSY.Hubs.ClientMethods
{
    public interface IMainClientMethods
    {
        Task ReceiveMessage(MessageModel message);
        Task ReceiveYourMessage(MessageModel message, int messageGUID);
        Task ReceiveChat(ReceiveChatModel chat);
    }
}
