
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ServerSide
{

    public class Program
    {

        public static void Main(string[] args)
        {
            MazeController Controller = new MazeController(8000);
            IClientHandler ch = new ClientHandler(Controller);
            Controller.MyClientHandler = ch;
            MazeModel model = new MazeModel(Controller);
            Controller.Model = model;
            Controller.InitializeCommands(); 
            Controller.Start();
            Console.Read();
        }

    }
}
