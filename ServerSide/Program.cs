
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ServerSide {

   public class Program
    {

        public static void Main(string[] args)
        {
            IView ch = new ClientHandler();
            IModel m = new MazeModel();
            IController c = new MazeController(5555,ch, m);
           
            //Server server = new Server(8000, new ClientHandler());
            c.Start();
            Console.Read();
        }
            
    }
}
