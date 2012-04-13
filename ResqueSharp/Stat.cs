using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ResqueSharp.Helper;
using ServiceStack.Redis;

namespace ResqueSharp
{
    public class Stat
    {
        private IRedisClient _redisClient;

        public Stat(IRedisClient redisClient)
        {
            _redisClient = redisClient;
        }

        public int Get(string stat)
        {
            return _redisClient.Get<int>(GetStatKey(stat));
        }

        public long Increment(string stat, uint amount)
        {
            return _redisClient.Increment(GetStatKey(stat), amount);
        }

        public long Increment(string stat)
        {
            return Increment(stat, 1);
        }

        public void Decrement(string stat)
        {
            _redisClient.Decrement(GetStatKey(stat), 1);
        }

        public void Clear(string stat)
        {
            _redisClient.Remove(GetStatKey(stat));
        }

        private static string GetStatKey(string stat)
        {
            return string.Format("{0}:stat:{1}", Constants.Namespace, stat);
        }
    }
}
