using ServerSide;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerSide
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
            TcpClient opp = model.GetOpponent(client);
            NetworkStream s = opp.GetStream();
            StreamWriter w = new StreamWriter(s);
            w.WriteLine(args[0]);
            w.Flush();
            return "YES";
        }
    }
}
