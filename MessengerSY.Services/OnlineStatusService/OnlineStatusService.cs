using MessengerSY.Core;
using MessengerSY.Data.Context;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerSY.Services.OnlineStatusService
{
    public class OnlineStatusService : IOnlineStatusService
    {
        private readonly IDatabase _redisDatabase;

        public OnlineStatusService(IMessengerRedisDbContext messengerRedisDbContext)
        {
            _redisDatabase = messengerRedisDbContext.Database;
        }

        public async Task SetOnlineStatus(int userProfileId)
        {
            if (userProfileId <= 0)
                throw new ArgumentException(nameof(userProfileId));

            var values = new HashEntry[]
            {
                new HashEntry(OnlineStatusConstants.IS_ONLINE, "true")
            };

            await _redisDatabase.HashSetAsync(userProfileId.ToString(), values);
        }

        public async Task SetOfflineStatus(int userProfileId)
        {
            if (userProfileId <= 0)
                throw new ArgumentException(nameof(userProfileId));

            var values = new HashEntry[]
            {
                new HashEntry(OnlineStatusConstants.IS_ONLINE, "false"),
                new HashEntry(OnlineStatusConstants.LAST_ONLINE_DATE, DateTime.Now.ToString())
            };

            await _redisDatabase.HashSetAsync(userProfileId.ToString(), values);
        }

        public async Task<bool> IsOnline(int userProfileId)
        {
            if (userProfileId <= 0)
                throw new ArgumentException(nameof(userProfileId));

            var value = await _redisDatabase.HashGetAsync(userProfileId.ToString(), OnlineStatusConstants.IS_ONLINE);

            if (value == "true")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<(bool isOnline, DateTime lastOnlineDate)> GetOnlineStatus(int userProfileId)
        {
            if (userProfileId <= 0)
                throw new ArgumentException(nameof(userProfileId));

            var valueIsOnline =
                await _redisDatabase.HashGetAsync(userProfileId.ToString(), OnlineStatusConstants.IS_ONLINE);

            var valueLastOnlineDate =
                await _redisDatabase.HashGetAsync(userProfileId.ToString(), OnlineStatusConstants.LAST_ONLINE_DATE);

            return (valueIsOnline == "true", DateTime.Parse(valueLastOnlineDate));
        }
    }
}
