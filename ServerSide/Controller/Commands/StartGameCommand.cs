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
        private MazeModel Model;


        /// <summary>
        /// Constructor, sets the model
        /// </summary>
        /// <param name="model"> model </param>
        public StartGameCommand(IModel model)
        {
            this.Model = model as MazeModel;
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
            Model.GenerateMaze(args[0], int.Parse(args[1]), int.Parse(args[2]), client, "Multi");
            StreamWriter S = new StreamWriter(client.GetStream());
            S.WriteLine("#");
            S.Flush();
            return "DO NOT CLOSE";
        }
    }
}