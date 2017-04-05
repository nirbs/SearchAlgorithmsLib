using System;
using System.Net.Sockets;

namespace MVC
{
    public class SolveMazeCommand : ICommand
    {
        private IModel model;

        public SolveMazeCommand(IModel model)
        {
            this.model = model;
        }

        public string Execute(string[] args, TcpClient client = null)
        {
            throw new NotImplementedException();
        }
    }
}