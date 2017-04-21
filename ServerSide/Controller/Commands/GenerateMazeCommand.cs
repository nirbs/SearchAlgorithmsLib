using ClientSide;
using MazeLib;
using ServerSide;
using ServerSide.View;
using System;
using System.IO;
using System.Net.Sockets;

namespace ServerSide
{
    /// <summary>
    /// Implements ICommand, to generate a maze and return to client
    /// </summary>
    internal class GenerateMazeCommand : ICommand
    {

        /// <summary>
        /// Private model member
        /// </summary>
        private MazeModel model;


        /// <summary>
        /// Constructor for command, receives model to work with
        /// </summary>
        /// <param name="model"> model which is cast to MazeModel</param>
        public GenerateMazeCommand(IModel model)
        {
            this.model = model as MazeModel;
        }

        /// <summary>
        /// Execute method for Generating a maze - generates it by requesting from model
        /// </summary>
        /// <param name="args"> name of new maze to be generated </param>
        /// <param name="client"> client that requested the maze to be generated </param>
        /// <returns> returns a string of the result of the execution </returns>
        public string Execute(string[] args, TcpClient client = null)
        {
            Maze maze = model.GenerateMaze(args[0], int.Parse(args[1]), int.Parse(args[2]), client, "Single");
            NetworkStream stream = client.GetStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.WriteLine(maze.ToJSON());
            writer.WriteLine("#");
            writer.Flush();
            return "CLOSE";
        }
    }
}