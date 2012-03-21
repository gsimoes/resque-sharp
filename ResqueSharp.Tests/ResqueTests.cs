using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ResqueSharp.Jobs;

namespace ResqueSharp.Tests
{
    public class ResqueTests
    {
        public static string RedisHost = "localhost";
        public static Resque ResqueClient = new Resque(RedisHost);

        public class PushMethod
        {
            [Test]
            public void ReturnsTrueIfJobCreated()
            {
                ResqueClient.Redis.FlushDb(); // Cleanup db

                var result = ResqueClient.Push("console", typeof(DummyJob).AssemblyQualifiedName, "Some message");
                var queues = ResqueClient.Queues();

                Assert.True(result);
                Assert.That(queues.Count() == 1 && queues.First().QueueName.Equals("console"));
                Assert.That(ResqueClient.QueueSize("console") == 1);
            }

            [Test]
            public void ThrowsArgumentExceptionIfQueueEmpty()
            {
                Assert.Throws<ArgumentException>(new TestDelegate(() => ResqueClient.Push("", typeof(DummyJob).AssemblyQualifiedName, "")));
            }

            [Test]
            public void ThrowsArgumentExceptionIfClassNameEmpty()
            {
                Assert.Throws<ArgumentException>(new TestDelegate(() => ResqueClient.Push("console", "", "")));
            }

            public void Create100Jobs()
            {
                for (int i = 0; i < 50; i++)
                {
                    ResqueClient.Push("console", typeof(DummyJob).AssemblyQualifiedName, "Some message");
                    ResqueClient.Push("failing", typeof(FailingJob).AssemblyQualifiedName);
                }
            }
        }

        public class PopMethod
        {
            [Test]
            public void ReturnsCorrectJob()
            {
                if (ResqueClient.Push("console", typeof(DummyJob).AssemblyQualifiedName, "Some message"))
                {
                    var result = ResqueClient.Pop("console");

                    Assert.True(result != null);
                    Assert.True(result.queue == "console");
                    Assert.True(result.payload.@class == typeof(DummyJob).AssemblyQualifiedName);
                }
            }
        }
    }
}
