using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonFiles.Config {
    public class ServicesConfig {
        public string Name { get; set; } = "";
        public string BashName { get; set; } = "";
        public int Port { get; set; } = 22;
    }
}
