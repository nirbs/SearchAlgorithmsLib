using MazeLib;
using ServerSide;
using System;
using System.Net.Sockets;

namespace MVC
{
    internal class GenerateMazeCommand : ICommand
    {
        private MazeModel model;

        public GenerateMazeCommand(IModel model)
        {
            this.model = model as MazeModel;
        }

        public string Execute(string[] args, TcpClient client = null)
        {
           Maze m =  model.GenerateMaze(args[0], int.Parse(args[1]), int.Parse(args[2]), client);

            return m.ToJSON();
        }
    }
}