using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logger
{
    public static class LogQueue
    {
        private static readonly ConcurrentQueue<LogQueueItem> _queue = new();

        public static void Enqueue(LogQueueItem item)
        {
            _queue.Enqueue(item);
        }

        public static bool TryDequeue(out LogQueueItem? item)
        {
            return _queue.TryDequeue(out item);
        }
    }

}
