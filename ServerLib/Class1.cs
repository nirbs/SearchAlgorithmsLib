﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLib
{
    public class Server
    {
        private int port;
        private TcpListener listener;
        private IClientHandler ch;

        public Server(int port, IClientHandler ch) { this.port = port; this.ch = ch; }

        public void Start()
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port); listener = new TcpListener(ep); listener.Start(); Console.WriteLine("Waiting for connections...");




        }
    }


}
