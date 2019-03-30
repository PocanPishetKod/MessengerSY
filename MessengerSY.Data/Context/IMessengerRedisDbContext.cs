using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessengerSY.Data.Context
{
    public interface IMessengerRedisDbContext
    {
        IDatabase Database { get; }
    }
}
