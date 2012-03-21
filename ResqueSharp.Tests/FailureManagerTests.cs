using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ResqueSharp.Jobs;

namespace ResqueSharp.Tests
{
    public class FailureManagerTests
    {
        public static string RedisHost = "localhost";
        public static Resque ResqueClient = new Resque(RedisHost);

        public class RequeueMethod
        {
            [Test]
            public void AddsNewJobToOriginalQueue()
            {
                ResqueClient.Redis.FlushDb(); // Cleanup db
                ResqueClient.Push("console", typeof(DummyJob).AssemblyQualifiedName, "some message");

                var worker = new Worker("localhost", "console");
                var job = worker.Reserve();

                Assert.That(ResqueClient.QueueSize("console") == 0);

                worker.Process(job);

                Assert.That(ResqueClient.FailureManager.Count() == 1);

                ResqueClient.FailureManager.Requeue(0);

                Assert.That(ResqueClient.QueueSize("console") == 1);
            }
        }
    }
}
