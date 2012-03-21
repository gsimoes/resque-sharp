using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using resque;
using System.Reflection;

namespace ResqueSharp.WorkerExample
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var worker = new Worker("localhost", "console", "failing"))
            {
                worker.Work();
            }
        }
    }
}
