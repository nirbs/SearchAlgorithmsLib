using System;
using System.Net.Sockets;

namespace MVC
{
    internal class ListAllGamesCommand : ICommand
    {
        private IModel model;

        public ListAllGamesCommand(IModel model)
        {
            this.model = model;
        }

        public string Execute(string[] args, TcpClient client = null)
        {
            throw new NotImplementedException();
        }
    }
}