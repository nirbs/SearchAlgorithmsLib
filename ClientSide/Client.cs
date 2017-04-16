using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientSide
{
    public class Client
    {
        public StreamWriter MyWriter { get; set; }
        public StreamReader MyReader { get; set; }
        public TcpClient MyTcp { get; set; }
        Dictionary<string, bool> commands;
        string CommandKey;


        public Client()
        {


            commands = new Dictionary<string, bool>
            { { "generate", true }, { "solve", true }, { "start", false }, { "list", false },
                { "join", false }, { "play", false }, { "close", true } };
        }


        public int ReceiveUpdates()
        {
            while (true)
            {
                string response = MyReader.ReadLine();
                while (MyReader.Peek() > 0)
                {
                    response += "\r\n";
                    response += MyReader.ReadLine();
                }


                Console.WriteLine("RESPONSE FROM SERVER:");
                Console.WriteLine(response);
            

                //Send again, or connection closes
                if (commands[CommandKey])
                {
                    MyTcp.Close();
                    
                    return 1;
                }
            }
        }

        public void SendUpdates()
        {
            string input = "";
            
            // string commandKey = "";
           
            while (true)
            {
                do
                {
                    input = Console.ReadLine();
                    string[] arr = input.Split(' ');
                    CommandKey = arr[0];

                } while (!commands.ContainsKey(CommandKey));
                //send input to Server
                MyWriter.WriteLine(input);
                MyWriter.Flush();
            }
        }
        public void BeginGame()
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5555);
            MyTcp = new TcpClient();
            MyTcp.Connect(ep);

            NetworkStream stream = MyTcp.GetStream();
            MyReader = new StreamReader(stream);
            MyWriter = new StreamWriter(stream);

            int val = 0;

            Task t1 = new Task(() =>
            {
                SendUpdates();
            });
            t1.Start();

            Task<int> t2 = new Task<int>(() =>
            {
                val = ReceiveUpdates();
                return val;
            });
            t2.Start();

            if (t2.Result == 1)
            {
                return;
            }
        }
    }

}