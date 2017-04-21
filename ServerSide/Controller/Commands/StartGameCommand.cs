using MazeLib;
using ServerSide;
using System;
using System.IO;
using System.Net.Sockets;

namespace ServerSide
{
    /// <summary>
    /// COmmand to start a new game
    /// </summary>
    public class StartGameCommand : ICommand
    {
        /// <summary>
        /// Private model member
        /// </summary>
        private MazeModel model;


        /// <summary>
        /// Constructor, sets the model
        /// </summary>
        /// <param name="model"> model </param>
        public StartGameCommand(IModel model)
        {
            this.model = model as MazeModel;
        }

        /// <summary>
        /// Generates a maze, adds the client as a player
        /// </summary>
        /// <param name="args"> name of game to start </param>
        /// <param name="client"> client to add as a player </param>
        /// <returns></returns>
        public string Execute(string[] args, TcpClient client = null)
        {
            //split up args
            model.GenerateMaze(args[0], int.Parse(args[1]), int.Parse(args[2]), client, "Multi");
            StreamWriter writer = new StreamWriter(client.GetStream());
            writer.WriteLine("#");
            writer.Flush();
            return "DO NOT CLOSE";
        }
    }
}