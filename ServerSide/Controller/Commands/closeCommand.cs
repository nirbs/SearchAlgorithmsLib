using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;

namespace ServerSide
{
    /// <summary>
    /// Implements ICommand, closes a specific game
    /// </summary>
    public class CloseCommand : ICommand
    {
        /// <summary>
        /// Private member of a Maze Model
        /// </summary>
        private MazeModel model;

        /// <summary>
        /// Constructor for CloseCommand
        /// </summary>
        /// <param name="model"> receives a model to work with </param>
        public CloseCommand(IModel model)
        {
            this.model = model as MazeModel;
        }

        /// <summary>
        /// Execute method for Close - closes both players in game
        /// </summary>
        /// <param name="args"> name of the maze game to close </param>
        /// <param name="client"> client who's game needs to be closed </param>
        /// <returns> returns a string of the result of the command's execution </returns>
        public string Execute(string[] args, TcpClient client = null)
        {
            string Name = args[0];

            //Gets the TCPClient that is playing against this client
            List<TcpClient> opponent = model.GetOpponents(client);
            model.EndGame(Name);
            string str;
            foreach (TcpClient O in opponent)
            {
                //Stream for opponent
                NetworkStream stream = O.GetStream();
                StreamWriter writer = new StreamWriter(stream);
                str = "Opponent closed game.\r\nGame ended";
                writer.WriteLine(str);
                writer.WriteLine("#");
                writer.Flush();
                writer = new StreamWriter(stream);
                writer.Flush();
            }

            //stream for this client
            NetworkStream net = client.GetStream();
            StreamWriter st = new StreamWriter(net);
            str = "Closing Game: " + Name;
            st.WriteLine(str);
            st.WriteLine("#");
            st.Flush();
            return "CLOSE";
        }
    }
}