using MazeLib;
using ServerSide;
using System;
using System.IO;
using System.Net.Sockets;

namespace ServerSide
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
            Maze m = model.GenerateMaze(args[0], int.Parse(args[1]), int.Parse(args[2]), client);
            Console.WriteLine(m.ToString());
            NetworkStream n = client.GetStream();
            StreamWriter w = new StreamWriter(n);
            w.WriteLine(m.ToString());
            w.Flush();
            return m.ToString();
        }
    }
}