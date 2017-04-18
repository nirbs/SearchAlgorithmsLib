using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SearchAlgorithmsLib;
using MazeLib;
using System.Net.Sockets;
using ServerSide.View;

namespace ServerSide
{


    public class MazeModel : IModel
    {
        IController controller;
        private Dictionary<string, MazeGame> mazeGames;
        private Dictionary<int, ISearcher<Position>> searchers;
        private Dictionary<string, Solution<Position>> solvedMazes;

        public MazeModel(IController c)
        {
            controller = c;

            //Dictionary to contain all mazeGames
            mazeGames = new Dictionary<string, MazeGame>();

            //Contains solutions to the mazes
            solvedMazes = new Dictionary<string, Solution<Position>>();

            searchers = new Dictionary<int, ISearcher<Position>>();

            //Adds searchers to dictionary
            searchers.Add(0, new BFS<Position>());
            searchers.Add(1, new DFS<Position>());
        }

        public Maze GenerateMaze(string name, int row, int col, TcpClient player)
        {
            //Creates the maze

            if(mazeGames.Keys.Contains(name))
            {
                mazeGames[name].player1 = player;
                return mazeGames[name].maze;
            }
            Maze newMaze = new MazeGeneratorLib.DFSMazeGenerator().Generate(row, col);
            newMaze.Name = name;

            //Adds the current client as player1 to this game
            MazeGame newGame = new MazeGame(newMaze, player);

            //Adds game to dictionary
            mazeGames.Add(name, newGame);

            Console.WriteLine("Maze {0} added to dictionary", name);
            Console.WriteLine("Current dictionary size: {0}", mazeGames.Count);
            return newMaze;
        }

        public string ListAllMazes()
        {
            List<string> keyList = new List<string>(this.mazeGames.Keys);
            Console.WriteLine( keyList);

            StringBuilder resultList = new StringBuilder();
            resultList.Append("[\r\n");
            


            //return a JSON of; all games in dictionary
            foreach(string key in mazeGames.Keys)
            {
                resultList.Append("\""+ key+"\",");
                resultList.Append("\r\n");
                
            }
            resultList.Append("]");
            Console.WriteLine(resultList.ToString());
            return resultList.ToString();
            /////////return "YES";
        }

        public Solution<Position> SolveMaze(string name, int searchType)
        {

            //Already solved this maze
            if (solvedMazes.ContainsKey(name))
            {
                Console.WriteLine("The maze '{0}' has already been solved...Returning solution", name);
                return solvedMazes[name];
            }

            //Selects wanted game from dictionary
            MazeGame game = mazeGames[name];
            Maze maze = game.maze;

            //Creates a new searchable with start and end point of selected maze
            MazeSearchable searchable = new MazeSearchable(new State<Position>(maze.InitialPos), new State<Position>(maze.GoalPos));

            //Sets the searchable's maze
            searchable.setMaze(maze);

            //selects the wanted search method
            ISearcher<Position> s = searchers[searchType];

            //solves the maze
            Solution<Position> sol = s.search(searchable);
            /*Console.WriteLine("Solution for maze:");
            foreach (State<Position> stat in sol.sol)
            {
                Console.WriteLine(stat.getState().ToString());
            }*/

            //Adds solution to Dictionary
            solvedMazes.Add(name, sol);

            //COnvert to JSON and return to client HERE OR IN COMMAND?
            return sol;

            
        }

        public void Move(TcpClient player)
        {
            //get other player and notify that this player moved using getOpponents
        }

        public MazeGame AddPlayer(string game, TcpClient player)
        {
            Console.WriteLine("Player added to {0}", game);
            mazeGames[game].AddPlayer(player);
            return mazeGames[game];
        }
        public Maze GenerateMaze(int row, int col)
        {
            return new MazeGeneratorLib.DFSMazeGenerator().Generate(row, col);

        }

        public MazeGame GetMaze(string name)
        {
            return mazeGames[name];
        }

        public TcpClient GetOpponent(TcpClient player)
        {
            foreach(MazeGame v in mazeGames.Values)
            {
                if (v.player1.Equals(player))
                    return v.player2;
                if (v.player2.Equals(player))
                    return v.player1;
            }
            return null;
        }

        public MazeGame GetGameOfClient(TcpClient client)
        {
            foreach(MazeGame game in mazeGames.Values)
            {
                if(game.player1 == client || game.player2 == client)
                {
                    return game;
                }
            }
            return null;
            
        }

    }

}