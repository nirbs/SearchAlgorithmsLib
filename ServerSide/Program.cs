
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
            MazeController controller = new MazeController(8000);
            ClientHandler ch= new ClientHandler(controller);
            controller.ch = ch;
            MazeModel model = new MazeModel(controller);
            controller.model = model;
            controller.createCommands(); 

            controller.Start();



            //Server server = new Server(8000, new ClientHandler());
           
            Console.Read();
        }

    }
}
