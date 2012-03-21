using System;

namespace ResqueSharp.Models
{
    public class FailureSummary
    {
        public DateTime Failed_at { get; set; }
        public Job.Payload Payload { get; set; }
        public string Error { get; set; }
        public string Backtrace { get; set; }
        public string Worker { get; set; }
        public string Queue { get; set; }
    }
}
