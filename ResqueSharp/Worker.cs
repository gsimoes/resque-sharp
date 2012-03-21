using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using ResqueSharp.Helper;
using ResqueSharp.Models;
using ServiceStack.Redis;

namespace ResqueSharp
{
    public class Worker : IDisposable
    {
        private string[] _queues;
        private Resque _resque;
        private string _workerId;

        public IRedisClient Redis
        {
            get { return _resque.Redis; }
        }

        public Worker(string redisHost, params string[] queues)
        {
            _resque = new Resque(redisHost);

            if (queues.Contains("*"))
                _queues = _resque.Queues().Select(s => s.QueueName.Split(':').Last()).ToArray();
            else
                this._queues = queues;
        }

        public void Work(int interval = 5)
        {
            try
            {
                RegisterWorker();

                while (true)
                {
                    var job = Reserve();

                    if (job != null)
                    {
                        Process(job);
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(interval * 1000);
                    }
                }
            }
            finally
            {
                UnregisterWorker();
            }
        }

        private void RegisterWorker()
        {
            this.Redis.AddItemToSet(Constants.WorkersSet, string.Format("worker:{0}", WorkerId()));
            this.Redis.Add(string.Format("worker:{0}:started", WorkerId()), DateTime.Now);
        }

        private void WorkingOn(Job job)
        {
            var data = new Dictionary<string, object>() { 
                { "queue", job.queue }, 
                { "run_at", DateTime.Now.ToString("ddd MMM dd HH:mm:ss zzzz yyyy", CultureInfo.CurrentCulture) }, 
                { "payload", job.payload } 
            };

            this.Redis.Add(string.Format("worker:{0}", WorkerId()), data);
        }

        private void DoneWorking(Job job)
        {
            this.Redis.Remove(string.Format("worker:{0}", WorkerId()));
        }

        private void UnregisterWorker()
        {
            this.Redis.RemoveItemFromSet(Constants.WorkersSet, string.Format("worker:{0}", WorkerId()));
            this.Redis.Remove(string.Format("worker:{0}:started", WorkerId()));
        }

        private void Process(Job job)
        {
            try
            {
                WorkingOn(job);
                job.Perform();
            }
            catch (Exception e)
            {
                _resque.LogFailure(new Failure(e.InnerException, this.WorkerId(), job.queue, job.payload));
            }
            finally
            {
                DoneWorking(job);
            }
        }

        private Job Reserve()
        {
            foreach (string queue in _queues)
            {
                var job = _resque.Pop(queue);

                if (job != null)
                {
                    return job;
                }
            }

            return null;
        }

        private string WorkerId()
        {
            if (_workerId == null)
            {
                _workerId = string.Format("{0}-{2}:{1}",
                    Dns.GetHostName(),
                    System.Diagnostics.Process.GetCurrentProcess().Id,
                    String.Join("-", _queues)
                );
            }

            return _workerId;
        }

        public void Dispose()
        {
            _resque.Dispose();
        }
    }
}
