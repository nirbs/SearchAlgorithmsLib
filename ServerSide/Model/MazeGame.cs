using ClientSide;
using MazeLib;
using ServerSide.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerSide
{
    public class MazeGame
    {

        public List<TcpClient> Players { get; set; }
       // public TcpClient player2 { get; set; }
        public Maze maze { get; set; } 
        public bool IsFull { get; set; }


        public MazeGame(Maze m)
        {
            Players = new List<TcpClient>();
            maze = m;
        }

        public void AddPlayer(TcpClient player)
        {
            Players.Add(player);
            
        }

        public bool HasPlayer(TcpClient player)
        {
            foreach(TcpClient P in Players)
            {
                if (player == P)
                    return true;
            }
            return false;
        }

        public List<TcpClient> GetOpponents(TcpClient player)
        {
            List<TcpClient> Opp = new List<TcpClient>(Players);
            Opp.Remove(player);
            return Opp;
        }



       
    }

   /*public class PlayEventArgs : EventArgs
    {
        public string Move { get; set; }
        public string Name { get; set; }
        public string ToString()
        {
            return "TO DO";
        }
        
    }*/
}
