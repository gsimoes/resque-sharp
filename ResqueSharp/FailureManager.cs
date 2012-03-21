using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ResqueSharp.Helper;
using ResqueSharp.Models;
using ServiceStack.Redis;

namespace ResqueSharp
{
    public class FailureManager
    {
        private IRedisClient _Redis;

        public FailureManager(IRedisClient redis)
        {
            _Redis = redis;
        }

        public void Save(Failure failure)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data.Add("failed_at", DateTime.Now);
            data.Add("payload", failure.Payload);
            data.Add("error", failure.Exception.Message);
            data.Add("backtrace", failure.Exception.ToString());
            data.Add("worker", failure.WorkerId);
            data.Add("queue", failure.Queue);

            var typedClient = _Redis.GetTypedClient<Dictionary<string, object>>();
            typedClient.AddItemToList(typedClient.Lists[Constants.FailuresList], data);
        }

        public List<FailureSummary> All(int start, int count = 1)
        {
            var typedClient = _Redis.GetTypedClient<FailureSummary>();

            return typedClient.GetRangeFromList(typedClient.Lists[Constants.FailuresList], start, start + count - 1);
        }

        public int Count()
        {
            return _Redis.GetListCount(Constants.FailuresList);
        }
    }
}
