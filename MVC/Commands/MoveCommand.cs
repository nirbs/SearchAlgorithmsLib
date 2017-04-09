using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MVC.Commands
{
    class MoveCommand : ICommand
    {
        MazeModel model;

        public MoveCommand(IModel m)
        {
            model = m as MazeModel;
        }
        public string Execute(string[] args, TcpClient client = null)
        {
            model.GetOpponent(client);
            return "YES";
        }
    }
}
