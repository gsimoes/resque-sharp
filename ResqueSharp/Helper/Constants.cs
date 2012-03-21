using System.Configuration;

namespace ResqueSharp.Helper
{
    public class Constants
    {
        public static string Namespace = ConfigurationManager.AppSettings["queue-namespace"] ?? "resque";
        public static string WorkersSet = string.Format("{0}:workers", Constants.Namespace);
        public static string FailuresList = string.Format("{0}:failures", Constants.Namespace);
    }
}
