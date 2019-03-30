using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MessengerSY.Services.OnlineStatusService
{
    public interface IOnlineStatusService
    {
        Task SetOnlineStatus(int userProfileId);
        Task SetOfflineStatus(int userProfileId);
        Task<bool> IsOnline(int userProfileId);
        Task<(bool isOnline, DateTime lastOnlineDate)> GetOnlineStatus(int userProfileId);
    }
}
