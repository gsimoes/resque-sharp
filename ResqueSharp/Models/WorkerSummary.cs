using System;

namespace ResqueSharp.Models
{
    public class WorkerSummary
    {
        public string WhereAt { get; set; }
        public string Queue { get; set; }
        public DateTime RunAt { get; set; }
        public string Job { get; set; }
    }
}
