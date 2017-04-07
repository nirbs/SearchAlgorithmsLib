using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MVC
{
    class ClientInfo
    {
        public bool HasPartner { get; set; }
        public TcpClient client { get; set; }
        public TcpClient partner { get; set; }
    }
}
