using System;

namespace ResqueSharp.Models
{
    public class Failure
    {
        public Exception Exception { get; set; }
        public string WorkerId { get; set; }
        public string Queue { get; set; }
        public object Payload { get; set; }

        public Failure(Exception exception, string workerId, String queue, Object payload)
        {
            this.Exception = exception;
            this.WorkerId = workerId;
            this.Queue = queue;
            this.Payload = payload;
        }
    }
}
