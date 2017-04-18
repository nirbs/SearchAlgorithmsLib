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
            bool check = false;

            // string commandKey = "";

            while (true)
            {
                do
                {
                    input = Console.ReadLine();

                    if (!checkValidity(input))
                    {
                        Console.WriteLine("Wrong command, please type again");
                        check = true;
                    }
                    else
                    {
                        check = false;
                    }

                } while (check);
                //send input to Server
                MyWriter.WriteLine(input);
                MyWriter.Flush();
            }
        }

        public bool checkValidity(string command)
        {
            string[] arr = command.Split(' ');
            CommandKey = arr[0];
            if (commands.ContainsKey(CommandKey))
            {
                switch (CommandKey)
                {
                    case "generate":
                        if (arr[2] != null && arr[3] != null)
                        {
                            return true;
                        }
                        return false;
                    case "solve":
                        if (arr[2] == "0" || arr[2] == "1")
                        {
                            return true;
                        }
                        return false;
                    case "start":
                        if (arr[2] != null && arr[3] != null)
                        {
                            return true;
                        }
                        return false;
                    case "list":
                        if (command.Length == CommandKey.Length)
                        {
                            return true;
                        }
                        return false;
                    case "join":
                        if (arr[1] != null)
                        {
                            return true;
                        }
                        return false;
                    case "play":
                        if (arr[1] == "right" || arr[1] == "left" || arr[1] == "up" || arr[1] == "down")
                        {
                            return true;
                        }
                        return false;
                    case "close":
                        if (arr[1] != null)
                        {
                            return true;
                        }
                        return false;
                    default:
                        return false;
                }

            }
            return false;
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
                this.BeginGame();
            }
        }
    }

}