
using ServerSide;
using ServerSide.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerSide
{
    class MazeController : IController
    {
        private Dictionary<string, ICommand> commands;
        public IModel model { get; set; }
        private int port;
        private TcpListener listener;
        public IClientHandler ch { get; set; }
        
        public MazeController(int port)
        {
            //ch = v;
            this.port = port;
            
            

            // more commands...
            
        }

        public void createCommands()
        {
            commands = new Dictionary<string, ICommand>();
            commands.Add("generate", new GenerateMazeCommand(model));
            commands.Add("solve", new SolveMazeCommand(model));
            commands.Add("start", new StartGameCommand(model));
            commands.Add("list", new ListAllGamesCommand(model));
            commands.Add("join", new JoinGameCommand(model));
            commands.Add("play", new MoveCommand(model));
            commands.Add("close", new closeCommand(model));
        }

        public string ExecuteCommand(string commandLine, TcpClient client)
        {
            string[] arr = commandLine.Split(' ');
            string commandKey = arr[0];
            if (!commands.ContainsKey(commandKey))
                return "Command not found";
            string[] args = arr.Skip(1).ToArray();
            ICommand command = commands[commandKey];
           // Player p = new Player(client);
            string results = command.Execute(args, client);
            //return command.Execute(args, client);
            return "YES";
        }

        public void Start()
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5555);
            listener = new TcpListener(ep);

            listener.Start();
            Console.WriteLine("Waiting for connections...");

            Task task = new Task(() =>
            {
                while (true)
                {
                    try
                    {
                        TcpClient client = listener.AcceptTcpClient();
                        Console.WriteLine("Got new connection");
                      
                        ch.HandleClient(client);
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

