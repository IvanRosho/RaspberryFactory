using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatusWorker {
    public class WorkerConfig {
        public int UpdateSystemInfo { get; set; }
        public int UpdateServiceInfo { get; set; }
        public int CountOfProcesses { get; set; }
    }
}
