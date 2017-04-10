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
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 23456);
            myTcp = new TcpClient();
            myTcp.Connect(ep);

            NetworkStream stream = myTcp.GetStream();
            myReader = new StreamReader(stream);
            myWriter = new StreamWriter(stream);

            while (true)
            {

                //Accept input
                string input = Console.ReadLine();

                //send input to Server
                myWriter.WriteLine(input);
                myWriter.Flush();

                //Receive input from server -- only reads first row, needs to read whole maze/response
                string response = myReader.ReadLine();
                do
                {
                    response += "\r\n";
                    response+= myReader.ReadLine();
                } while (myReader.Peek() > 0);
               

                Console.WriteLine("RESPONSE FROM SERVER:");
                Console.WriteLine(response);

                //Send again, or connection closes

            }
        }

    }
}