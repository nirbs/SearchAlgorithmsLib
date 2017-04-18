using MazeLib;
using ServerSide;
using System;
using System.Net.Sockets;

namespace ServerSide
{
    /// <summary>
    /// 
    /// </summary>
    public class StartGameCommand : ICommand
    {
        private MazeModel model;


        public StartGameCommand(IModel model)
        {
            this.model = model as MazeModel;
        }

        public string Execute(string[] args, TcpClient client = null)
        {
            //split up args
            model.GenerateMaze(args[0], int.Parse(args[1]), int.Parse(args[2]), client);
            return "YES";
        }
    }
}