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

       // public delegate void OpponentMovedEventHandler(object game, PlayEventArgs playerMove);

       // public event OpponentMovedEventHandler OpponentMoved;

        //////protected virtual void OnOpponenetMoved(string move)
      //  {
         ////   OpponentMoved?.Invoke(this, new PlayEventArgs() { Move = move, Name = maze.Name });
      //  }

        public TcpClient player1 { get; set; }
        public TcpClient player2 { get; set; }
        public Maze maze { get; set; }


        public MazeGame(Maze m, TcpClient p1)
        {
            player1 = p1;
            maze = m;
        }

        public void AddPlayer(TcpClient p2)
        {
            player2 = p2;
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
