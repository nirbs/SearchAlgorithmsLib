﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerSide
{
    public class ClientHandler : IView
    {
        private IController controller;


        public void HandleClient(TcpClient client)
        {
            new Task(() =>
            {
                NetworkStream stream = client.GetStream();
    
                StreamReader reader = new StreamReader(stream);
                StreamWriter writer = new StreamWriter(stream);
                while (true) {
                    string commandLine = reader.ReadLine();
                    Console.WriteLine("Got command: {0}", commandLine);

                    string result = controller.ExecuteCommand(commandLine, client);

                }
            }).Start();
        }
    }
}