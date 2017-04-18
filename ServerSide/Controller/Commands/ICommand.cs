using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerSide
{
    /// <summary>
    /// Interface for commands for controller to execute
    /// </summary>
    interface ICommand
    {
        /// <summary>
        /// method for each command to implement
        /// </summary>
        /// <param name="args"> varies according to command </param>
        /// <param name="client"> client which is requesting command </param>
        /// <returns> returns a string with the result of the executions </returns>
        string Execute(string[] args, TcpClient client = null);
    }
}
