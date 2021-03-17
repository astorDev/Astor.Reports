using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Astor.Reports.Protocol
{
    public class EventNames
    {
        public const string Created = "created";
        
        public const string BuildStarted = "buildStarted";

        public const string Done = "done";

        public static IEnumerable<string> GetAll() => typeof(EventNames).GetFields().Select(p => (string)p.GetRawConstantValue());
    }
}