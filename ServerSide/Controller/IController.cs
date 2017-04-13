﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerSide
{
   public interface IController
    {
        string ExecuteCommand(string commandLine, TcpClient client);
        void Start();
    }
}
