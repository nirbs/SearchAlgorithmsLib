using System;
using System.Net.Sockets;

namespace MVC
{
    public class SolveMazeCommand : ICommand
    {
        private MazeModel model;

        public SolveMazeCommand(IModel model)
        {
            this.model = model;
        }

        public string Execute(string[] args, TcpClient client = null)
        {

            string solution = model.solveMaze(args);
            //send to client
            //close client
        }
    }
}