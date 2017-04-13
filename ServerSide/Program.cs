
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
            Server server = new Server(8000, new ClientHandler());
            server.Start();
            Console.Read();
        }

    }
}
