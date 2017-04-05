using System;
using System.Net.Sockets;

namespace MVC
{
    public class closeCommand : ICommand
    {
        private IModel model;

        public closeCommand(IModel model)
        {
            this.model = model;
        }

        public string Execute(string[] args, TcpClient client = null)
        {
            throw new NotImplementedException();
        }
    }
}