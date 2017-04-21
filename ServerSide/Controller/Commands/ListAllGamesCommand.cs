using System;
using System.IO;
using System.Net.Sockets;

namespace ServerSide
{
    /// <summary>
    /// Lists all the games currently in play
    /// </summary>
    internal class ListAllGamesCommand : ICommand
    {
        /// <summary>
        /// private member model
        /// </summary>
        private MazeModel Model;


        /// <summary>
        /// Constructor, sets the model
        /// </summary>
        /// <param name="model"> model to work with </param>
        public ListAllGamesCommand(IModel model)
        {
            Model = model as MazeModel;
        }

        /// <summary>
        /// Returns a list of the multi player games
        /// </summary>
        /// <param name="args"> rempty arguments </param>
        /// <param name="client"> client requesting the list of games </param>
        /// <returns> results of execution </returns>
        public string Execute(string[] args, TcpClient client = null)
        {
            string AllGames =  Model.ListAllMazes();
            NetworkStream Stream = client.GetStream();
            StreamWriter Writer = new StreamWriter(Stream);
            Writer.WriteLine(AllGames);
            Writer.WriteLine("#");
            Writer.Flush();
            return "CLOSE";
        }
    }
}