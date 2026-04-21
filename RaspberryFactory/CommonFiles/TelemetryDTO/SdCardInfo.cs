    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonFiles.TelemetryDTO {
    public class SdCardInfo
    {
        public List<SdPartition> Partitions { get; set; } = new();
    }

    public class SdPartition
    {
        public string Mount { get; set; } = "";
        public string Filesystem { get; set; } = "";
        public int SizeMb { get; set; }
        public int UsedMb { get; set; }
        public int AvailableMb { get; set; }
        public int UsedPercent { get; set; }
    }

}
