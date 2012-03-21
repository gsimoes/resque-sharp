using System.Collections.Generic;
using ResqueSharp.Models;

namespace ResqueSharp.Web.Models
{
    public class WorkersViewModel
    {      
        public List<WorkerSummary> Workers { get; set; }
        public int TotalWorkers { get; set; }
    }    
}