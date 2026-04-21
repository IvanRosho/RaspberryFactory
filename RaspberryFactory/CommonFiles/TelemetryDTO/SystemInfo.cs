using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonFiles.TelemetryDTO {
    public class SystemInfo
    {
        public double CpuLoad { get; set; }
        public RamInfo Ram { get; set; } = new();
        public SdCardInfo SdCard { get; set; } = new();
    }

}
