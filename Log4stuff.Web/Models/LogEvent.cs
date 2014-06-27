using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace Log4stuff.Web.Models
{
    public class LogEvent
    {
        public string Logger { get; set; }

        public string Thread { get; set; }

        public DateTime Timestamp { get; set; }

        public string Level { get; set; }

        public string Message { get; set; }

        public string ApplicationId { get; set; }

        public Dictionary<string, string> Metadata { get; private set; }

        public LogEvent()
        {
            Metadata = new Dictionary<string, string>();
        }
    }
}