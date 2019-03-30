using MessengerSY.Models.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerSY.Hubs.ClientMethods
{
    public interface INotificationClientMethods
    {
        Task ReceiveMessage(MessageModel message);
    }
}
