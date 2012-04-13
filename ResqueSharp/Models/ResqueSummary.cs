namespace ResqueSharp.Models
{
    public class ResqueSummary
    {
        public int Pending { get; set; }
        public int Processed { get; set; }
        public int Queues { get; set; }
        public int Workers { get; set; }
        public int Working { get; set; }
        public int Failed { get; set; }
    }
}
