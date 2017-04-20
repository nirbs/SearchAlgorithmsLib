using ServerSide.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerSide
{
    /// <summary>
    /// Interface for controller job
    /// </summary>
   public interface IController
    {
        /// <summary>
        /// Method to execute a requested command
        /// </summary>
        /// <param name="commandLine"> command to execute </param>
        /// <param name="client"> client requesting </param>
        /// <returns> results from execution </returns>
        string ExecuteCommand(string commandLine, TcpClient client);

        /// <summary>
        /// To initializes commands
        /// </summary>
        void InitializeCommands();
        /// <summary>
        /// Method to start execution 
        /// </summary>
        void Start();

        void SetModel(IModel m);


        void SetView(IView v);
    }
}
