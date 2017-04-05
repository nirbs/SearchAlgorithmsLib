using System;
using System.Net.Sockets;

namespace MVC
{
    public class PlayOneMoveCommand : ICommand
    {
        private IModel model;

        public PlayOneMoveCommand(IModel model)
        {
            this.model = model;
        }

        public string Execute(string[] args, TcpClient client = null)
        {
            throw new NotImplementedException();
        }
    }
}