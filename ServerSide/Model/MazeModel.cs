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
        private Dictionary<string, MazeGame> MultiPlayerGames;
        private Dictionary<string, MazeGame> SinglePlayerGames;
        private Dictionary<int, ISearcher<Position>> Searchers;
        private Dictionary<string, Solution<Position>> SolvedMazes;

        public MazeModel(IController c)
        {
            controller = c;

            //Dictionary to contain all mazeGames
            SinglePlayerGames = new Dictionary<string, MazeGame>();
            MultiPlayerGames = new Dictionary<string, MazeGame>();


            //Contains solutions to the mazes
            SolvedMazes = new Dictionary<string, Solution<Position>>();

            Searchers = new Dictionary<int, ISearcher<Position>>();

            //Adds searchers to dictionary
            Searchers.Add(0, new BFS<Position>());
            Searchers.Add(1, new DFS<Position>());
        }

        public Maze GenerateMaze(string name, int row, int col, TcpClient player, string gameType)
        {
            //Creates the maze
            Maze NewMaze = new MazeGeneratorLib.DFSMazeGenerator().Generate(row, col);
            NewMaze.Name = name;

            //Adds the current client as player1 to this game
            MazeGame NewGame = new MazeGame(NewMaze);

            //Adds game to dictionary

            if (gameType == "Single")
            {
                SinglePlayerGames.Add(name, NewGame);
            } else
            {
                Console.WriteLine("generated Multi Game");
                NewGame.AddPlayer(player);
                MultiPlayerGames.Add(name, NewGame);
                Console.WriteLine(MultiPlayerGames[name].maze.Name);
                
            }

           // mazeGames.Add(name, newGame);

            //Console.WriteLine("Maze {0} added to dictionary", name);
            //Console.WriteLine("Current dictionary size: {0}", mazeGames.Count);
            return NewMaze;
        }

        public string ListAllMazes()
        {
            List<string> KeyList = new List<string>(MultiPlayerGames.Keys);
           // Console.WriteLine( keyList);

            StringBuilder resultList = new StringBuilder();
            resultList.Append("[\r\n");



            //return a JSON of; all games in dictionary
            Console.Write("Size of dictionary: {0}", MultiPlayerGames.Keys.Count);
            foreach(string key in MultiPlayerGames.Keys)
            {
                resultList.Append("\"" + key + "\"");
                if (MultiPlayerGames[key].IsFull)
                {
                    resultList.Append("->GAME FULL");
                }
                
                resultList.Append(",\r\n");
                
            }
            resultList.Append("]");
            Console.WriteLine(resultList.ToString());
            return resultList.ToString();
        }

        public Solution<Position> SolveMaze(string name, int searchType)
        {

            //Already solved this maze
            if (SolvedMazes.ContainsKey(name))
            {
                Console.WriteLine("The maze '{0}' has already been solved...Returning solution", name);
                return SolvedMazes[name];
            }

            //Selects wanted game from dictionary
            MazeGame Game;
            if (SinglePlayerGames.Keys.Contains(name))
            {
                Game = SinglePlayerGames[name];
            } else
            {
                 Game = MultiPlayerGames[name];
            }
           
            Maze MazeToSolve = Game.maze;

            //Creates a new searchable with start and end point of selected maze
            MazeSearchable Searchable = new MazeSearchable(new State<Position>(MazeToSolve.InitialPos), new State<Position>(MazeToSolve.GoalPos));

            //Sets the searchable's maze
            Searchable.setMaze(MazeToSolve);

            //selects the wanted search method
            ISearcher<Position> MazeSearcher = Searchers[searchType];

            //solves the maze
            Solution<Position> Sol = MazeSearcher.search(Searchable);
            
            /*Console.WriteLine("Solution for maze:");
            foreach (State<Position> stat in sol.sol)
            {
                Console.WriteLine(stat.getState().ToString());
            }*/

            //Adds solution to Dictionary
            SolvedMazes.Add(name, Sol);

            //COnvert to JSON and return to client HERE OR IN COMMAND?
            return Sol;

            
        }

        public void Move(TcpClient player)
        {
            //get other player and notify that this player moved using getOpponents
        }

        public MazeGame AddPlayer(string game, TcpClient player)
        {
            Console.WriteLine("Player added to {0}", game);
            if(!MultiPlayerGames[game].IsFull)
                MultiPlayerGames[game].AddPlayer(player);
            //mazeGames[game].AddPlayer(player);
            return MultiPlayerGames[game];
        }



        public Maze GenerateMaze(int row, int col)
        {
            return new MazeGeneratorLib.DFSMazeGenerator().Generate(row, col);

        }

        public MazeGame GetMaze(string name)
        {
            if(SinglePlayerGames.Keys.Contains(name))
            {
                return SinglePlayerGames[name];
            } else
            {
                return MultiPlayerGames[name];
            }
        }

        public List<TcpClient> GetOpponents(TcpClient player)
        {
            List<TcpClient> Opponents = new List<TcpClient>();
            foreach (MazeGame v in MultiPlayerGames.Values)
            {
                if (v.HasPlayer(player))
                    Opponents = v.GetOpponents(player);
            }
            return Opponents;
        }

        public MazeGame GetGameOfClient(TcpClient client)
        {
            foreach(MazeGame Game in MultiPlayerGames.Values)
            {
                if(Game.HasPlayer(client))
                {
                    return Game;
                }
            }
            return null;
            
        }

    }

}