using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonFiles.TelemetryDTO {
    public class ServiceStatus
    {
        public string Status { get; set; } = "";
        public int Port { get; set; }
        public bool PortOpen { get; set; }
    }
}
