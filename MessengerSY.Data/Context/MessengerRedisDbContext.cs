using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessengerSY.Data.Context
{
    public class MessengerRedisDbContext : IMessengerRedisDbContext
    {
        private ConnectionMultiplexer _connectionMultiplexer;
        private IDatabase _redisDb;

        public IDatabase Database
        {
            get
            {
                if (_redisDb == null)
                {
                    _connectionMultiplexer = ConnectionMultiplexer.Connect(new ConfigurationOptions()
                    {
                        AllowAdmin = true,
                        ConnectTimeout = 60 * 1000,
                        EndPoints = { { "localhost", 6379 } }
                    });
                    _redisDb = _connectionMultiplexer.GetDatabase();
                }

                return _redisDb;
            }
        }
    }
}
