
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
            IController controller = new MazeController(8000);
            IView ch = new ClientHandler(controller);
            controller.SetView(ch);
            IModel model = new MazeModel(controller);
            controller.SetModel(model);
            controller.InitializeCommands();
            controller.Start();
            Console.Read();
        }

    }
}
