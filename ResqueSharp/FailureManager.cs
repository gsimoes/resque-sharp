using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using ResqueSharp.Helper;
using ResqueSharp.Models;
using ServiceStack.Redis;

namespace ResqueSharp
{
    public class FailureManager
    {
        private Resque _Resque;

        public FailureManager(Resque resque)
        {
            _Resque = resque;
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

            var typedClient = _Resque.Redis.GetTypedClient<Dictionary<string, object>>();
            typedClient.AddItemToList(typedClient.Lists[Constants.FailuresList], data);
        }

        public List<FailureSummary> All(int start, int count = 1)
        {
            var typedClient = _Resque.Redis.GetTypedClient<FailureSummary>();

            return typedClient.GetRangeFromList(typedClient.Lists[Constants.FailuresList], start, start + count - 1);
        }

        public void Requeue(int index)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            var typedClient = _Resque.Redis.GetTypedClient<Dictionary<string, object>>();
            var item = typedClient.GetItemFromList(typedClient.Lists[Constants.FailuresList], index);

            item["retried_at"] = DateTime.Now;
            typedClient.SetItemInList(typedClient.Lists[Constants.FailuresList], index, item);

            _Resque.Push(item["queue"].ToString(), ((Job.Payload)item["payload"]).@class, ((Job.Payload)item["payload"]).args);
        }

        //   def self.requeue(index)
        //  item = all(index)
        //  item['retried_at'] = Time.now.strftime("%Y/%m/%d %H:%M:%S")
        //  Resque.redis.lset(:failed, index, Resque.encode(item))
        //  Job.create(item['queue'], item['payload']['class'], *item['payload']['args'])
        //end

        public int Count()
        {
            return _Resque.Redis.GetListCount(Constants.FailuresList);
        }
    }
}
