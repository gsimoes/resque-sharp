using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ResqueSharp.Helper;
using ResqueSharp.Models;
using ServiceStack.Redis;
using ServiceStack.Text;

namespace ResqueSharp
{
    public class Resque : IDisposable
    {
        public RedisClient Redis
        {
            get;
            private set;
        }

        public FailureManager FailureManager
        {
            get;
            set;
        }

        public Resque(string host)
        {
            this.Redis = new RedisClient(host);
            this.FailureManager = new FailureManager(this);
        }

        public bool Push(string queue, string className, params object[] args)
        {
            try
            {
                WatchQueue(queue);

                return Job.Create(
                    queue,
                    className,
                    job => {
                        var typedRedis = Redis.GetTypedClient<Job>();
                        typedRedis.EnqueueItemOnList(typedRedis.Lists[string.Format("{0}:queue:{1}", Constants.Namespace, queue)], job);
                    },
                    args
                );
            }
            catch (RedisException)
            {
                return false;
            }
        }

        public Job Pop(string queue)
        {
            var typedClient = Redis.GetTypedClient<Job>();
            var list = typedClient.Lists[string.Format("{0}:queue:{1}", Constants.Namespace, queue)];

            return typedClient.DequeueItemFromList(list);
        }

        public int QueueSize(string queue)
        {
            return Redis.GetListCount(string.Format("{0}:queue:{1}", Constants.Namespace, queue));
        }

        public int WorkersCount()
        {
            return Redis.SearchKeys("worker:*:started").Count();
        }

        public void LogFailure(Failure failure)
        {
            this.FailureManager.Save(failure);
        }

        public List<QueueSummary> Queues()
        {
            return Redis
                .GetAllItemsFromSet(string.Format("{0}:queues", Constants.Namespace))
                .Select(queue => new QueueSummary {
                    QueueName = queue,
                    Jobs = QueueSize(queue)
                })
                .ToList();
        }

        public List<WorkerSummary> WorkersRunningJobs()
        {
            return this.Redis
                  .GetAllItemsFromSet(Constants.WorkersSet)
                  .Select(w => new { worker = w, data = Redis.GetValue(w) })
                  .Where(w => w.data != null)
                  .Select(s => {
                      var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(s.data);

                      return new WorkerSummary {
                          WhereAt = s.worker.Substring(s.worker.IndexOf(":") + 1),
                          RunAt = DateTime.ParseExact(data["run_at"].ToString(), "ddd MMM dd HH:mm:ss zzzz yyyy", CultureInfo.CurrentCulture),
                          Job = ((JValue)((JContainer)data["payload"])["class"]).Value.ToString().Split(',').First(),
                          Queue = data["queue"].ToString()
                      };
                  })
                  .ToList();
        }

        private void WatchQueue(string queue)
        {
            Redis.AddItemToSet(string.Format("{0}:queues", Constants.Namespace), queue); // Adds the queue to set of queues
        }

        public void Dispose()
        {
            if (Redis != null)
                Redis.Dispose();

            Redis = null;
        }
    }
}

