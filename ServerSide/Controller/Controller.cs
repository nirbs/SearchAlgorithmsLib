using ServerSide.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerSide.Controller
{
    abstract class Controller : IController
    {
        IModel model;
        IView clientHandler;

        public abstract string ExecuteCommand(string commandLine, TcpClient client);
        public abstract void InitializeCommands();
        public abstract void SetModel(IModel m);
        public abstract void SetView(IView v);
        public abstract void Start();
    }
}
