﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MVC
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

                //While loop added so this task will constantly take this clients input
                while (true)
                {
                    {
                        string commandLine = reader.ReadLine();
                        //Console.WriteLine("MESSAGE FROM CLIENT: {0}", commandLine);
                        string result = controller.ExecuteCommand(commandLine, client);
                    }
                }
            }).Start();
        }
    }
}
