
using ServerSide;
using ServerSide.View;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerSide
{

    /// <summary>
    /// Implementation of IController, for a maze Controller
    /// </summary>
    class MazeController : IController
    {
        /// <summary>
        /// a dictionary to contain all commands for controller
        /// </summary>
        private Dictionary<string, ICommand> commands;
        /// <summary>
        /// Model member
        /// </summary>
        public IModel Model { get; set; }
        /// <summary>
        /// Port number for controller
        /// </summary>
        private int Port;
        /// <summary>
        /// Listener member for TCP
        /// </summary>
        private TcpListener Listener;
        /// <summary>
        /// Client handler to handle clients
        /// </summary>
        public IClientHandler MyClientHandler { get; set; }
        
        /// <summary>
        /// Contructor for Maze Controller, sets the port
        /// </summary>
        /// <param name="port"></param>
        public MazeController(int port)
        {
            Port = port;
        }
        /// <summary>
        /// Creates all the commands for the controller
        /// </summary>
        public void InitializeCommands()
        {
            commands = new Dictionary<string, ICommand>();
            commands.Add("generate", new GenerateMazeCommand(Model));
            commands.Add("solve", new SolveMazeCommand(Model));
            commands.Add("start", new StartGameCommand(Model));
            commands.Add("list", new ListAllGamesCommand(Model));
            commands.Add("join", new JoinGameCommand(Model));
            commands.Add("play", new MoveCommand(Model));
            commands.Add("close", new CloseCommand(Model));
        }

        /// <summary>
        /// Executes a given command using its dictionary
        /// </summary>
        /// <param name="commandLine"> command to perform from user </param>
        /// <param name="client"> client to perform command for </param>
        /// <returns>Returns a string</returns>
        public string ExecuteCommand(string commandLine, TcpClient client)
        {
            string[] arr = commandLine.Split(' ');
            string commandKey = arr[0];
            if (!commands.ContainsKey(commandKey))
                return "Command not found";
            string[] args = arr.Skip(1).ToArray();
            ICommand command = commands[commandKey];
            string results = command.Execute(args, client);
            return results;
        }

        /// <summary>
        /// Method to begin controller's job. Creates tasks for every client
        /// that connects and sends the client to the client handler
        /// </summary>
        public void Start()
        {
            string listenPort = ConfigurationManager.AppSettings["port"];
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Int32.Parse(listenPort));
            Listener = new TcpListener(ep);

            Listener.Start();
            Console.WriteLine("Waiting for connections...");

            Task task = new Task(() =>
            {
                while (true)
                {
                    try
                    {
                        TcpClient client = Listener.AcceptTcpClient();
                        Console.WriteLine("New Player Connected");
                        MyClientHandler.HandleClient(client);
                    }
                    catch (SocketException)
                    {
                        break;
                    }
                }
                Console.WriteLine("Server stopped");
            });
            task.Start();

        }
    }
    }

