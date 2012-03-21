using System;

namespace ResqueSharp.Models
{
    public class Failure
    {
        public Exception Exception { get; set; }
        public string WorkerId { get; set; }
        public string Queue { get; set; }
        public Job.Payload Payload { get; set; }

        public Failure(Exception exception, string workerId, String queue, Job.Payload payload)
        {
            this.Exception = exception;
            this.WorkerId = workerId;
            this.Queue = queue;
            this.Payload = payload;
        }
    }
}
