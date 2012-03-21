using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Reflection;

namespace ResqueSharp
{
    public class Job
    {
        public string queue { get; set; }
        public Payload payload { get; set; }

        public Job() { }

        public Job(string queue, Payload payload)
        {
            this.queue = queue;
            this.payload = payload;
        }

        internal void Perform()
        {
            var methodInfo = Type.GetType((string)payload.@class, true)
                .GetMethod("Perform", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);

            if (methodInfo == null)
                throw new NotImplementedException();

            methodInfo.Invoke(null, payload.args);
        }

        public static bool Create(string queue, string className, Action<Job> onSuccess, params object[] args)
        {
            if (String.IsNullOrEmpty(queue))            
                throw new ArgumentException(string.Format("Queue can not be empty: {0}", queue));            

            if (String.IsNullOrEmpty(className))            
                throw new ArgumentException(string.Format("ClassName can not be empty: {0}", className));            

            var job = new Job(queue, new Payload { @class = className, args = args });

            onSuccess(job);

            return true;
        }

        public class Payload
        {
            public string @class { get; set; }
            public object[] args { get; set; }

            public string Class()
            {
                return this.@class;
            }
        }
    }
}

