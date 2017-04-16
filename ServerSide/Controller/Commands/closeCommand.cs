using System;
using System.IO;
using System.Net.Sockets;

namespace ServerSide
{
    public class closeCommand : ICommand
    {
        private MazeModel model;

        public closeCommand(IModel model)
        {
            this.model = model as MazeModel;
        }

        public string Execute(string[] args, TcpClient client = null)
        {
            string name = args[0];
            TcpClient opp = model.GetOpponent(client);
            NetworkStream n = opp.GetStream();
            StreamWriter s = new StreamWriter(n);

            
            NetworkStream net = client.GetStream();
            StreamWriter st = new StreamWriter(net);

            string str = "opponent closed game.\r\nGame ended";
            s.WriteLine(str);
            s.Flush();
            str = "closing game";
            st.WriteLine(str);
            st.Flush();

            return "YES";
        }
    }
}