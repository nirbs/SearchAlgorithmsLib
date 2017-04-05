using System;
using System.Net.Sockets;

namespace MVC
{
    internal class GenerateMazeCommand : ICommand
    {
        private IModel model;

        public GenerateMazeCommand(IModel model)
        {
            this.model = model;
        }

        public string Execute(string[] args, TcpClient client = null)
        {
                
        }
    }
}