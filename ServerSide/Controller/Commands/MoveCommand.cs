using Newtonsoft.Json.Linq;
using ServerSide;
using ServerSide.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerSide
{
    /// <summary>
    /// command to move a step in the maze
    /// </summary>
    class MoveCommand : ICommand
    {
        /// <summary>
        /// private member of model
        /// </summary>
        MazeModel model;

        /// <summary>
        /// constructor that sets model
        /// </summary>
        /// <param name="model"></param>
        public MoveCommand(IModel model)
        {
            this.model = model as MazeModel;

        }

        /// <summary>
        /// Notifies the client's opponent of his move in the game
        /// </summary>
        /// <param name="args"> the clients move </param>
        /// <param name="client"> client that moved</param>
        /// <returns></returns>
        public string Execute(string[] args, TcpClient client = null)
        {
            List<TcpClient> opponents = model.GetOpponents(client);
            if (opponents.Count == 0)
            {
                return "";
            }
            string jsonString = Json(model.GetGameOfClient(client).Maze.Name, args[0]);

            foreach (TcpClient Opponent in opponents)
            {
                NetworkStream stream = Opponent.GetStream();
                StreamWriter writer = new StreamWriter(stream);
                //Turns the move into a JSON to send to opponent
                writer.WriteLine(jsonString);
                writer.WriteLine("#");
                writer.Flush();
            }
            StreamWriter s = new StreamWriter(client.GetStream());
            s.WriteLine("#");
            s.Flush();
            return "DO NOT CLOSE";
        }

        /// <summary>
        /// method to turn the name of the game and direction into a JSON
        /// </summary>
        /// <param name="name"> name of maze </param>
        /// <param name="movement"> direction of movement </param>
        /// <returns> returns string in JSON form </returns>
        public string Json(string name, string movement)
        {
            JObject jsonString = new JObject
            {
                ["Name"] = name,
                ["Direction"] = movement,
            };
            return jsonString.ToString();
        }


    }


}
