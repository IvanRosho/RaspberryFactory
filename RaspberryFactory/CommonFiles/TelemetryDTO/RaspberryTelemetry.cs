using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonFiles.TelemetryDTO {
    public class RaspberryTelemetry {
        public string RaspberryName { get; set; } = "";
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public SystemInfo System { get; set; } = new();
        public List<ProcessInfo> TopProcesses { get; set; } = new();
    }

}
