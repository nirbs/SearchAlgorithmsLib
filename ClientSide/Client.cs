using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
        string MyInput = "";
        Thread ThreadToCancel;
        bool NotReadyToContinue = true;
        bool CheckClose = false;
        string newInput = "empty";

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
            while (true)
            {
                string response = MyReader.ReadLine();
                string closeCheck = "Opponent closed game.\r\nGame ended";
                while (MyReader.Peek() > 0)
                {
                    response += "\r\n";
                    response += MyReader.ReadLine();
                }
                Console.WriteLine(response);
                if (response.Equals(closeCheck))
                {
                    MyTcp.Close();
                    while(NotReadyToContinue)
                    {

                    }
                   // MyTcp = new TcpClient();
                }
                
                /*  string response = MyReader.ReadLine();
                  string closeCheck = "Opponent closed game.\r\nGame ended";
                  while (MyReader.Peek() > 0)
                  {
                      response += "\r\n";
                      response += MyReader.ReadLine();
                  }
                  Console.WriteLine("RESPONSE FROM SERVER:");
                  Console.WriteLine(response);
                  if (response.Equals(closeCheck))
                  {
                      CheckClose = true;
                  }
                  //Send again, or connection closes
                  if (commands[CommandKey] || CheckClose)
                  {
                      MyTcp.Close();
                      return 1;
                  }*/
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
        /// Method to be able to always send messages to server
        /// </summary>
        public void SendUpdates()
        {
            while (true)
            {
                MyInput = Console.ReadLine();
                if (!checkValidity(MyInput))
                {
                    Console.WriteLine("Wrong command, please type again");
                    continue;
                }
                if(!MyTcp.Connected)
                {
                    OpenSocket();
                    NotReadyToContinue = false;
                    
                }
                MyWriter.WriteLine(MyInput);
                MyWriter.Flush();
            }
        }




                /*
                ThreadToCancel = Thread.CurrentThread;
                string input = "";
                bool CommandCheck = false;

                while (true)
                {
                    do
                    {
                        if (newInput == "empty")
                        {
                            Console.WriteLine("inside:");
                            Console.WriteLine("CheckClose = {0}", CheckClose);
                            Console.Write("Please insert something: ");
                            if(CheckClose)
                            {
                               // newInput = input;
                                Console.WriteLine("Input before exiting: {0}", MyInput);
                                CheckClose = false;
                                Console.WriteLine("Returning from Send Task");
                                return;
                            }
                                MyInput = Console.ReadLine();
                            if (!checkValidity(MyInput))
                            {
                                Console.WriteLine("Wrong command, please type again");
                                CommandCheck = true;
                            }
                            else
                            {
                                CommandCheck = false;
                                if (CheckClose)
                                {
                                    newInput = MyInput;
                                    Console.WriteLine("Input before exiting: {0}", newInput);
                                    CheckClose = false;
                                    Console.WriteLine("Returning from Send Task");
                                    return;
                                }
                            }
                        }
                        else
                        {
                            input = MyInput;
                            Console.WriteLine("Last else, input is to send: {0}", input);
                            newInput = "empty";
                            CommandCheck = false;
                        }

                    } while (CommandCheck);

                    //send input to Server
                    MyWriter.WriteLine(input);
                    MyWriter.Flush();
                    if (commands[CommandKey] || CheckClose)
                    {
                        return;
                    }
                }*/
            



        /// <summary>
        /// Begins the logic of the client
        /// </summary>
        public int BeginGame()
        {
            ThreadToCancel = null;

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
           
            if(ThreadToCancel != null)
               ThreadToCancel.Abort();
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

          while (true)
              {
                  int check = BeginGame();

              }
            /* int check = BeginGame();
             if (check==1)
             {
                 while (true)
                 {
                     if (newInput == "empty")
                     {
                         Console.WriteLine("outside:");
                         newInput = Console.ReadLine();
                         if (checkValidity(newInput))
                         {
                             this.BeginGame();
                         }
                         else
                         {
                             Console.WriteLine("Wrong command, please type again");
                         }
                     }
                     else
                     {
                         BeginGame();
                     }

                 }
             }
             else
             {
                 Console.WriteLine("not 1 not good");
             }
         }
        }*/
        }
    }

}