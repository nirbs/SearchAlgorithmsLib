using ServerSide;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;

namespace ServerSide
{
    /// <summary>
    /// Command to join a requested game
    /// </summary>
    public class JoinGameCommand : ICommand
    {

        /// <summary>
        /// Private model member
        /// </summary>
        private MazeModel Model;

        /// <summary>
        /// Constructor which sets the model
        /// </summary>
        /// <param name="model"> MazeModel to work with </param>
        public JoinGameCommand(IModel model)
        {
            Model = model as MazeModel;
        }

        /// <summary>
        /// Execution requests from model to add this client to a
        /// given game, with the name args[0], and returns the maze
        /// to both of the players
        /// </summary>
        /// <param name="args"> name of maze to join </param>
        /// <param name="client"> client who wants to join the game </param>
        /// <returns> returns the result of the execution </returns>
        public string Execute(string[] args, TcpClient client = null)
        {
            //args[0] is name of game to join

            //Returns the game that this player joined
            MazeGame game = Model.AddPlayer(args[0], client);
            game.IsFull = true;
            List<TcpClient> Players = game.Players;
            foreach(TcpClient P in Players)
            {
                NetworkStream Stream = P.GetStream();
                StreamWriter Writer = new StreamWriter(Stream);
                Writer.WriteLine(game.Maze.ToJSON());
                Writer.WriteLine("#");
                Writer.Flush();
            }
            return "DO NOT CLOSE";
        }
    }
}