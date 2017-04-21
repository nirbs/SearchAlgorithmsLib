using ServerSide.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerSide
{
    /// <summary>
    /// Class to handle the clients once they are connected to the server
    /// </summary>
    public class ClientHandler : IView
    {
        /// <summary>
        /// Controller member
        /// </summary>
        private IController controller;
        /// <summary>
        /// Contructor which sets the controller
        /// </summary>
        /// <param name="c">controller</param>
        public ClientHandler(IController c)
        {
            controller = c;
        }

        /// <summary>
        /// Method to handle the client - opens a new
        /// task that performs the command and sends it to client
        /// </summary>
        /// <param name="client"> client being served </param>
        public void HandleClient(TcpClient client)
        {
            new Task(() =>
            {
                NetworkStream stream = client.GetStream();
                StreamReader reader = new StreamReader(stream);
                StreamWriter writer = new StreamWriter(stream);
                while (true)
                {
                    string commandLine = reader.ReadLine();
                    string result = controller.ExecuteCommand(commandLine, client);
                    writer.WriteLine(result);
                    writer.Flush();
                }
            }).Start();
        }
    }
}