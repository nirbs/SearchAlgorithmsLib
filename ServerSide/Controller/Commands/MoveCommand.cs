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
    class MoveCommand : ICommand
    {
        MazeModel model;
        
        public MoveCommand(IModel m)
        {
            model = m as MazeModel;
           
        }
        public string Execute(string[] args, TcpClient client = null)
        {
            TcpClient opp = model.GetOpponent(client);

            NetworkStream n = opp.GetStream();
            StreamWriter s = new StreamWriter(n);
            ////s.WriteLine(m.ToJSON());
            //s.Flush();
            string jsonString = Json(model.GetGameOfClient(client).maze.Name, args[0]);
            s.WriteLine(jsonString);
            s.Flush();
            return "YES";
        }

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
