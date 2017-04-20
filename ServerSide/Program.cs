
using ServerSide.View;
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
            IController Controller = new MazeController(8000);
            IView ch = new ClientHandler(Controller);
            Controller.SetView(ch);
            IModel model = new MazeModel(Controller);
            Controller.SetModel(model);
            Controller.InitializeCommands(); 
            Controller.Start();
            Console.Read();
        }

    }
}
