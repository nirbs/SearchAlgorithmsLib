﻿using System;
using System.Net.Sockets;

namespace MVC
{
    public class JoinGameCommand : ICommand
    {
        private IModel model;

        public JoinGameCommand(IModel model)
        {
            this.model = model;
        }

        public string Execute(string[] args, TcpClient client = null)
        {
            throw new NotImplementedException();
            
        }
    }
}