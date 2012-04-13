using System;
using System.Threading;

namespace ResqueSharp.Jobs
{
    public class DummyJob : Job
    {
        public static void Perform(string message)
        {
            Thread.Sleep(1000);
            Console.WriteLine(message);
        }
    }

    public class FailingJob : Job
    {
        public static void Perform()
        {
            Thread.Sleep(2000);
            throw new ArgumentException("Some error has occured in you app!");
        }
    }
}
