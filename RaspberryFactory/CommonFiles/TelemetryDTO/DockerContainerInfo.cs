using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonFiles.TelemetryDTO
{
    public class DockerContainerInfo
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Image { get; set; } = "";
        public string Status { get; set; } = "";
        public string Ports { get; set; } = "";
        public string Created { get; set; } = "";
    }

}
