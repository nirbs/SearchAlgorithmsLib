using ServerSide;
using System;
using System.IO;
using System.Net.Sockets;

namespace ServerSide
{
    public class JoinGameCommand : ICommand
    {
        private MazeModel model;

        public JoinGameCommand(IModel model)
        {
            this.model = model as MazeModel;
        }

        public string Execute(string[] args, TcpClient client = null)
        {
            //args[0] is name of game to join
            
            //Returns the game that this player joined
            MazeGame game = model.AddPlayer(args[0], client);
            NetworkStream stream = game.player1.GetStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.WriteLine(game.maze.ToJSON());
            writer.Flush();
            stream = game.player2.GetStream();
            writer = new StreamWriter(stream);
            writer.WriteLine(game.maze.ToJSON());
            writer.Flush();

            return "YES";
        }
    }
}