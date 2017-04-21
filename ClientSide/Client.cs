using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

using System.Net;
using System.Net.Sockets;

using System.Threading.Tasks;

namespace ClientSide
{
    /// <summary>
    /// Client class
    /// </summary>
    public class Client
    {
        /// <summary>
        /// Stream writer member
        /// </summary>
        public StreamWriter MyWriter { get; set; }
        /// <summary>
        /// StreamReader member
        /// </summary>
        public StreamReader MyReader { get; set; }
        /// <summary>
        /// Tcp member
        /// </summary>
        public TcpClient MyTcp { get; set; }
        /// <summary>
        /// Dictionary of valid commands for user to input
        /// </summary>
        Dictionary<string, bool> commands;
        /// <summary>
        /// Current command key
        /// </summary>
        string CommandKey;
        /// <summary>
        /// Client's input  
        /// </summary>
        string MyInput = "";
        /// <summary>
        /// Tells recieveUpdates task when a new connection is made
        /// </summary>
        bool NotReadyToContinue = true;

        /// <summary>
        /// Constructor for Client, initializes valid commands
        /// </summary>
        public Client()
        {
            commands = new Dictionary<string, bool>
            { { "generate", true }, { "solve", true }, { "start", false }, { "list", false },
                { "join", false }, { "play", false }, { "close", true } };
        }

        /// <summary>
        /// Method to constantly be able to receive messages from server
        /// </summary>
        /// <returns> returns a value to know if the user closed </returns>
        public int ReceiveUpdates(Task sendingTask)
        {
            string temp = "";
            while (true)
            {
                string response = MyReader.ReadLine();
                if (!response.Equals("#"))
                {
                    while (MyReader.Peek() > 0)
                    {
                        response += "\r\n";
                        temp = MyReader.ReadLine();
                        if (temp.Contains("#"))
                        {
                            break;
                        }
                        response += temp;
                    }
                    Console.WriteLine(response);
                }
                string close = MyReader.ReadLine();
                if (close.Equals("CLOSE"))
                {
                    MyTcp.Close();
                    OpenSocket();
                    NotReadyToContinue = false;
                }
                else
                {
                    NotReadyToContinue = false;
                }
            }
        }

        /// <summary>
        /// Method to be able to always send messages to server
        /// </summary>
        public void SendUpdates()
        {
            while (true)
            {
                //if(!NotReadyToContinue)
                MyInput = Console.ReadLine();

                if (!CheckValidity(MyInput))
                {
                    Console.WriteLine("Wrong command, please type again");
                    continue;
                }
                string[] arr = MyInput.Split(' ');
                CommandKey = arr[0];

                MyWriter.WriteLine(MyInput);
                MyWriter.Flush();
                while (NotReadyToContinue)
                {

                }
                NotReadyToContinue = true;
            }
        }

        /// <summary>
        /// Checks if the user input is a valid request
        /// </summary>
        /// <param name="command"> requested command </param>
        /// <returns> valid or not </returns>
        public bool CheckValidity(string command)
        {
            string[] arr = command.Split(' ');
            CommandKey = arr[0];
            if (commands.ContainsKey(CommandKey))
            {
                switch (CommandKey)
                {
                    case "generate":
                        if (arr.Length == 4)
                        {
                            return true;
                        }
                        return false;
                    case "solve":
                        if (arr.Length == 3 && (arr[2] == "0" || arr[2] == "1"))
                        {
                            return true;
                        }
                        return false;
                    case "start":
                        if (arr.Length == 4)
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
                        if (arr.Length == 2)
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
                        if (arr.Length == 2)
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

        /// <summary>
        /// Begins the logic of the client
        /// </summary>
        public int BeginGame()
        {
            OpenSocket();
            int val = 0;

            //Task to send messages
            Task t1 = new Task(() =>
            {

                //ThreadToCancel = Thread.CurrentThread;
                SendUpdates();
                return;
            });
            t1.Start();

            //Task to receive messages
            Task<int> t2 = new Task<int>(() =>
            {
                val = ReceiveUpdates(t1);

                return val;
            });
            t2.Start();

            return t2.Result;
        }

        /// <summary>
        /// Creates a new socket and opens a connection to the server
        /// </summary>
        public void OpenSocket()
        {
            string clientPort = ConfigurationManager.AppSettings["port"];
            string serverIp = ConfigurationManager.AppSettings["ip"];
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(serverIp), Int32.Parse(clientPort));
            MyTcp = new TcpClient();
            MyTcp.Connect(ep);

            NetworkStream stream = MyTcp.GetStream();
            MyReader = new StreamReader(stream);
            MyWriter = new StreamWriter(stream);
        }

        /// <summary>
        /// Method that is called from main and calls the BeginGame method whenever the connection is closed
        /// </summary>
        public void StartAll()
        {

            while (true)
            {
                int check = BeginGame();
            }
        }
    }

}