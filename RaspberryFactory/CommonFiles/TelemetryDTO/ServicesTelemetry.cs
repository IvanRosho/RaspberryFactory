using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonFiles.TelemetryDTO {
    public class ServicesTelemetry
    {
        public string RaspberryName { get; set; } = "";
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public Dictionary<string, ServiceStatus> Services { get; set; } = new();
        public List<DockerContainerInfo> DockerContainers { get; set; } = new();
        public List<ListeningPort> ListeningPorts { get; set; } = new();
    }

}
