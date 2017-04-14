using MazeLib;
using System;
using System.IO;
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
            Console.WriteLine(m.ToJSON());
            NetworkStream n = client.GetStream();
            StreamWriter s = new StreamWriter(n);
            s.WriteLine(m.ToJSON());
            s.Flush();
            return m.ToString();
        }
    }
}