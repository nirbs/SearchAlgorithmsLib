using System;
using System.IO;
using System.Net.Sockets;

namespace ServerSide
{
    internal class ListAllGamesCommand : ICommand
    {
        private MazeModel model;

        public ListAllGamesCommand(IModel model)
        {
            this.model = model as MazeModel;
        }

        public string Execute(string[] args, TcpClient client = null)
        {
            string AllGames =  model.ListAllMazes();
            NetworkStream n = client.GetStream();
            StreamWriter s = new StreamWriter(n);
            s.WriteLine(AllGames);
            s.Flush();
            return "YES";
        }
    }
}