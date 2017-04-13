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
        Dictionary<string, bool> commands;

        public Client()
        {
            commands = new Dictionary<string, bool>
            { { "generate", true }, { "solve", true }, { "start", false }, { "list", false },
                { "join", false }, { "play", false }, { "close", false } };
        }

        public void BeginGame()
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 23456);
            myTcp = new TcpClient();
            myTcp.Connect(ep);

            NetworkStream stream = myTcp.GetStream();
            myReader = new StreamReader(stream);
            myWriter = new StreamWriter(stream);
            string input = "";
            string commandKey;
            while (true)
            {
                do
                {
                    input = Console.ReadLine();
                    string[] arr = input.Split(' ');
                    commandKey = arr[0];

                } while (!commands.ContainsKey(commandKey));
                //send input to Server
                myWriter.WriteLine(input);
                myWriter.Flush();
                

                //Receive input from server -- only reads first row, needs to read whole maze/response
                string response = myReader.ReadLine();
                while (myReader.Peek() > 0) 
                {
                    response += "\r\n";
                    response+= myReader.ReadLine();
                } 
               

                Console.WriteLine("RESPONSE FROM SERVER:");
                Console.WriteLine(response);

                //Send again, or connection closes
                if (commands[commandKey])
                {
                    myTcp.Close();
                    return;
                }
                
            }
        }

    }
}