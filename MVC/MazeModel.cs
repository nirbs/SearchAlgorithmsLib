using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SearchAlgorithmsLib;
using MazeLib;
using System.Net.Sockets;

namespace MVC { 


    public class MazeModel : IModel
    {       
        private Dictionary<string, MazeGame> mazeGames;        
        
        public MazeModel()
        {
            mazeGames = new Dictionary<string, MazeGame>();
        }

        public void AddMaze(string name, int row, int col, TcpClient client)
        {
            Maze newMaze =  new MazeGeneratorLib.DFSMazeGenerator().Generate(row, col);
            MazeGame newGame = new MazeGame(newMaze, client);
            mazeGames.Add(name, newGame);
        }

        public string ListAllMazes()
        {
            //return a JSON of all games in dictionary
        }

        public string solveMaze(string[] mazeNameAndAlgo)
        {
            //return solution, then command closes client
        }

        public void move(TcpClient player)
        {
            //get other player and notify that this player moved
        }

        public void AddPlayer(string game, TcpClient player)
        {
            //add player to a game from dictionary
        }

    }

}

