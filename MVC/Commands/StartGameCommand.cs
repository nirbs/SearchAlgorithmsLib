using System;
using System.Net.Sockets;

namespace MVC
{
    public class StartGameCommand : ICommand
    {
        private IModel model;


        public StartGameCommand(IModel model)
        {
            this.model = model;
        }

        public string Execute(string[] args, TcpClient client = null)
        {
            //TODO Create clientInfo ? and add to model?
        }
    }
}