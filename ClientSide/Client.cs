using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
        /// DIctionary of valid commands for user to input
        /// </summary>
        Dictionary<string, bool> commands;
        /// <summary>
        /// Current command key
        /// </summary>
        string CommandKey;

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

        /// <summary>
        /// Method to be able to always send messages to server
        /// </summary>
        public void SendUpdates(string newInput)
        {
            string input = "";
            bool check = false;

            while (true)
            {
                do
                {
                    if (newInput=="empty")
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
                    }
                    else
                    {
                        input = newInput;
                        newInput = "empty;";
                        check = false;
                    }

                } while (check);

                //send input to Server
                MyWriter.WriteLine(input);
                MyWriter.Flush();
                if (commands[CommandKey])
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Checks if the user input is a valid request
        /// </summary>
        /// <param name="command"> requested command </param>
        /// <returns> valid or not </returns>
        public bool checkValidity(string command)
        {
            string[] arr = command.Split(' ');
            CommandKey = arr[0];
            if (commands.ContainsKey(CommandKey))
            {
                switch (CommandKey)
                {
                    case "generate":
                        if (arr.Length==4)
                        {
                            return true;
                        }
                        return false;
                    case "solve":
                        if (arr.Length==3 && (arr[2] == "0" || arr[2] == "1"))
                        {
                            return true;
                        }
                        return false;
                    case "start":
                        if (arr.Length==4)
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
                        if (arr.Length==2)
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
        public int BeginGame(string input)
        {
            
            OpenSocket();

            

            int val = 0;

            //Task to send messages
            Task t1 = new Task(() =>
            {
               SendUpdates(input);
                return;
            });
            t1.Start();

            //Task to receive messages
            Task<int> t2 = new Task<int>(() =>
            {
                val = ReceiveUpdates();
                
                return val;
            });
            t2.Start();

            return t2.Result;
            /*if (t2.Result == 1)
            {
                while (true)
                {
                    string newInput = Console.ReadLine();
                    if (checkValidity(newInput))
                    {
                        this.BeginGame(newInput);
                    }
                    
                }
            }*/
        }

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

        public void startAll()
        {
            int check = BeginGame("empty");
            if (check==1)
            {
                while (true)
                {
                    string newInput = Console.ReadLine();
                    if (newInput.Equals("list"))
                    {
                        Console.WriteLine("listttttt");
                    }
                    if (checkValidity(newInput))
                    {
                        this.BeginGame(newInput);
                    }
                    else
                    {
                        Console.WriteLine("Wrong command, please type again");
                    }

                }
            }
            else
            {
                Console.WriteLine("not 1 not good");
            }
        }
    }

}