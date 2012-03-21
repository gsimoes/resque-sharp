using System.Collections.Generic;
using ResqueSharp.Models;

namespace ResqueSharp.Web.Models
{
    public class QueuesViewModel
    {
        public List<QueueSummary> Queues { get; set; }
        public int FailedCount { get; set; }
    }
}