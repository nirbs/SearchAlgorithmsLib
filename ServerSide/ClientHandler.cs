using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerSide
{
    public class ClientHandler : IClientHandler
    {
        private IController controller = new Controller();

        public void HandleClient(TcpClient client)
        {
            new Task(() =>
            {
            NetworkStream stream = client.GetStream();
            StreamReader reader = new StreamReader(stream);
                StreamWriter writer = new StreamWriter(stream);
                {
                    string commandLine = reader.ReadLine();
                    Console.WriteLine("Got command: {0}", commandLine);

                    string result = controller.ExecuteCommand(commandLine, client);
                }
            }).Start();
        }
    }
}
