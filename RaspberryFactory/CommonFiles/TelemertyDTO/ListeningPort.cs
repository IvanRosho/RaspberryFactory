using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonFiles.TelemertyDTO
{
    public class ListeningPort
    {
        public string Protocol { get; set; } = "";
        public int Port { get; set; }
        public string Process { get; set; } = "";
    }

}
