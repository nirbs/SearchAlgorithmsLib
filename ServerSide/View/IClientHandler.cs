using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerSide
{
    /// <summary>
    /// Interface for classes which handle Clients
    /// </summary>
    public interface IClientHandler
    {
        /// <summary>
        /// Method to handle a client
        /// </summary>
        /// <param name="client"> client to handle</param>
        void HandleClient(TcpClient client);
    }
}
