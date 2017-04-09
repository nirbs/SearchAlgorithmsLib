using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientSide
{
    public class Client
    {
        TcpClient myTcp;
        StreamReader myReader;
        StreamWriter myWriter;

        public Client()
        {


        }

        public void BeginGame()
        {
            while(true)
            {

                IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5555);
                myTcp = new TcpClient();
                myTcp.Connect(ep);

                NetworkStream stream = myTcp.GetStream();
                myReader = new StreamReader(stream);
                myWriter = new StreamWriter(stream);


                //Accept input
                // string input = Console.ReadLine();

                //send input to Server
                myWriter.Write("HELLO FROM CLIENT");

                //Receive input from server
                string response =  myReader.ReadLine();
                Console.WriteLine("RESPONSE FROM SERVER: {0}",response);
                //Send again, or connection closes
            }
        }

    }
}
