using MazeLib;
using SearchAlgorithmsLib;
using ServerSide;
using System;
using System.Net.Sockets;

namespace ServerSide
{
    public class SolveMazeCommand : ICommand
    {
        private MazeModel model;

        public SolveMazeCommand(IModel model)
        {
            this.model = model as MazeModel;
        }

        public string Execute(string[] args, TcpClient client = null)
        {
            //Solves an existing maze
            SearchAlgorithmsLib.Solution<Position> solution = model.SolveMaze(args[0], int.Parse(args[1]));

            //Send to client

            //Close client
            //client.Close();
            return solution.ToString();
        }
    }
}