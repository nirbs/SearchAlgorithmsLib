using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
/*
namespace ClientSide
{
    public class Player
    {
        public StreamWriter MyWriter { get; set; }
        public StreamReader MyReader { get; set; }
        public TcpClient MyTcp { get; set; }
        Dictionary<string, bool> commands;
        string CommandKey;

        *  public void OnOpponentMoved(object sender, PlayEventArgs playerMove)
          {
              Console.WriteLine(playerMove.Move);
          }
          

        public Player()
        {
            TcpClient client = new TcpClient();
            NetworkStream n = client.GetStream();
            MyReader = new StreamReader(n);
            MyWriter = new StreamWriter(n);
            MyTcp = client;

            commands = new Dictionary<string, bool>
            { { "generate", true }, { "solve", true }, { "start", false }, { "list", false },
                { "join", false }, { "play", false }, { "close", false } };
        }

        public void ReceiveUpdates()
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
                    return;
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
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 23456);
            MyTcp = new TcpClient();
            MyTcp.Connect(ep);

            NetworkStream stream = MyTcp.GetStream();
            MyReader = new StreamReader(stream);
            MyWriter = new StreamWriter(stream);

            new Task(() =>
            {
                SendUpdates();
            }).Start();

            new Task(() =>
            {
                ReceiveUpdates();
            }).Start();
        }
    }
}*/