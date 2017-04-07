using MazeLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MVC
{
    class MazeGame
    {
        private TcpClient player1;
        private TcpClient player2;
        private Maze maze;

        public MazeGame(Maze m, TcpClient p1)
        {
            player1 = p1;
            maze = m;
        }

        public void AddPlayer(TcpClient p2)
        {
            player2 = p2;
        }

        public TcpClient GetPartner(TcpClient player)
        {
            //return other player
        } 
    }
}
