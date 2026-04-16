using Logger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logger
{
    public class LogQueueItem
    {
        public LogItemType Type { get; set; }
        public Log? Log { get; set; }
        public LogError? LogError { get; set; }
    }
}
