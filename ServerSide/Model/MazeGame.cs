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

    /// <summary>
    /// class to hold players of a certain game 
    /// </summary>
    public class MazeGame
    {
        /// <summary>
        /// List of all players of this game
        /// </summary>
        public List<TcpClient> Players { get; set; }
        /// <summary>
        /// Maze of the game
        /// </summary>
        public Maze Maze { get; set; }
        /// <summary>
        /// Property to check if the game is full or
        /// if they can still add a player
        /// </summary>
        public bool IsFull { get; set; }

        /// <summary>
        /// Constructor - sets the maze
        /// </summary>
        /// <param name="m"> maze </param>
        public MazeGame(Maze m)
        {
            Players = new List<TcpClient>();
            Maze = m;
        }
        /// <summary>
        /// Adds a player to the game
        /// </summary>
        /// <param name="player"> player to add </param>
        public void AddPlayer(TcpClient player)
        {
            Players.Add(player);
        }
        /// <summary>
        /// Method to check if a player belongs to this game
        /// </summary>
        /// <param name="player"> player to check </param>
        /// <returns> if the player belongs to the game </returns>
        public bool HasPlayer(TcpClient player)
        {
            foreach (TcpClient P in Players)
            {
                if (player == P)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the opponents of a player from his game
        /// </summary>
        /// <param name="player"> Player to get its opponents </param>
        /// <returns> List of opponents </returns>
        public List<TcpClient> GetOpponents(TcpClient player)
        {
            List<TcpClient> opponents = new List<TcpClient>(Players);
            opponents.Remove(player);
            return opponents;
        }
    }
}
