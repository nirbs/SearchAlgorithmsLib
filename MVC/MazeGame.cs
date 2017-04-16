using MazeLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MVC
{
    public class MazeGame
    {
        public TcpClient player1 { get; set; }
        public TcpClient player2 { get; set; }
        public MazeLib.Maze maze { get; set; }


        public MazeGame(MazeLib.Maze m, TcpClient p1)
        {
            player1 = p1;
            maze = m;
        }

        public void AddPlayer(TcpClient p2)
        {
            player2 = p2;
        }

    }
}
