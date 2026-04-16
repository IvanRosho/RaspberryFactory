using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonFiles.TelemertyDTO
{
    public class RootTelemetry
    {
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public SystemInfo System { get; set; } = new();
        public List<ProcessInfo> TopProcesses { get; set; } = new();
        public Dictionary<string, ServiceStatus> Services { get; set; } = new();
        public List<DockerContainerInfo> DockerContainers { get; set; } = new();
        public List<ListeningPort> ListeningPorts { get; set; } = new();
    }

}
