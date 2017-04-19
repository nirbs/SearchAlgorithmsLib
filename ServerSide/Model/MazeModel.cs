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

    /// <summary>
    /// Specific implementation of IModel, to contain the maze game logic
    /// </summary>
    public class MazeModel : IModel
    {

        /// <summary>
        /// Controller member
        /// </summary>
        IController Controller;

        /// <summary>
        /// Dictionary to contain multi player games
        /// </summary>
        private Dictionary<string, MazeGame> MultiPlayerGames;

        /// <summary>
        /// Dictionary to contain single player games
        /// </summary>
        private Dictionary<string, MazeGame> SinglePlayerGames;

        /// <summary>
        /// Dictionary with all types of searchers
        /// </summary>
        private Dictionary<int, ISearcher<Position>> Searchers;

        /// <summary>
        /// Dictionary of all mazes that have already been solved
        /// </summary>
        private Dictionary<string, Solution<Position>> SolvedMazes;


        /// <summary>
        /// Constructor which sets the controller and
        /// initializes all the dictionaries
        /// </summary>
        /// <param name="c"> controller </param>
        public MazeModel(IController c)
        {
            Controller = c;

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

        /// <summary>
        /// Method to generate a maze and add it to a dictionary
        /// </summary>
        /// <param name="name"> name of the new maze to generate </param>
        /// <param name="row"> number of rows </param>
        /// <param name="col"> number of columns </param>
        /// <param name="player"> client that wants to create the maze </param>
        /// <param name="gameType"> single or multi-player maze </param>
        /// <returns> Returns the new maze </returns>
        public Maze GenerateMaze(string name, int row, int col, TcpClient player, string gameType)
        {
            //Creates the maze
            Maze NewMaze = new MazeGeneratorLib.DFSMazeGenerator().Generate(row, col);
            NewMaze.Name = name;

            //Adds the current client as player1 to this game
            MazeGame NewGame = new MazeGame(NewMaze);

            if (gameType == "Single")
            {
                SinglePlayerGames.Add(name, NewGame);
            }
            else //Multi Player
            {
                Console.WriteLine("generated Multi Game");
                NewGame.AddPlayer(player);
                MultiPlayerGames.Add(name, NewGame);
                Console.WriteLine(MultiPlayerGames[name].Maze.Name);

            }
            return NewMaze;
        }


        /// <summary>
        /// Method to return a list of all multi player games
        /// </summary>
        /// <returns> list of names of mazes </returns>
        public string ListAllMazes()
        {
            List<string> KeyList = new List<string>(MultiPlayerGames.Keys);
            StringBuilder resultList = new StringBuilder();
            resultList.Append("[\r\n");

            //return a JSON of; all games in dictionary
            foreach (string key in MultiPlayerGames.Keys)
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

        /// <summary>
        /// Solves a requested maze
        /// </summary>
        /// <param name="name"> name of maze to solve </param>
        /// <param name="searchType"> type of search to perform </param>
        /// <returns> Solution for the maze </returns>
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
           
            Maze MazeToSolve = Game.Maze;

            //Creates a new searchable with start and end point of selected maze
            MazeSearchable Searchable = new MazeSearchable(new State<Position>(MazeToSolve.InitialPos), new State<Position>(MazeToSolve.GoalPos));

            //Sets the searchable's maze
            Searchable.setMaze(MazeToSolve);

            //selects the wanted search method
            ISearcher<Position> MazeSearcher = Searchers[searchType];

            //solves the maze
            Solution<Position> Sol = MazeSearcher.search(Searchable);
            
            //Adds solution to Dictionary
            SolvedMazes.Add(name, Sol);

            //Convert to JSON and return to client HERE OR IN COMMAND?
            return Sol;
        }

        public void Move(TcpClient player)
        {
            //get other player and notify that this player moved using getOpponents
        }

        /// <summary>
        /// Adds a player to a requested game
        /// </summary>
        /// <param name="game"> name of game to join </param>
        /// <param name="player"> player that wants to join </param>
        /// <returns> returns the Maze Game that was just joined </returns>
        public MazeGame AddPlayer(string game, TcpClient player)
        {
            if(!MultiPlayerGames[game].IsFull)
                MultiPlayerGames[game].AddPlayer(player);
            return MultiPlayerGames[game];
        }

        /// <summary>
        /// gets a maze generator
        /// </summary>
        /// <param name="row"> how many rows </param>
        /// <param name="col"> how many columns</param>
        /// <returns> the new maze generator </returns>
        public Maze GenerateMaze(int row, int col)
        {
            return new MazeGeneratorLib.DFSMazeGenerator().Generate(row, col);
        }

        /// <summary>
        /// Gets the maze Game of a given name
        /// </summary>
        /// <param name="name"> which game to return </param>
        /// <returns> the requested game </returns>
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


        /// <summary>
        /// Gets all players playing against 'player'
        /// </summary>
        /// <param name="player"> current player </param>
        /// <returns> list of all opponents </returns>
        public List<TcpClient> GetOpponents(TcpClient player)
        {
            List<TcpClient> Opponents = new List<TcpClient>();
            //Finds the game that player belongs to, and returns its other players
            foreach (MazeGame v in MultiPlayerGames.Values)
            {
                if (v.HasPlayer(player))
                    Opponents = v.GetOpponents(player);
            }
            return Opponents;
        }


        /// <summary>
        /// Returns the game of given client
        /// </summary>
        /// <param name="client"> client</param>
        /// <returns> the clients game</returns>
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

        /// <summary>
        /// Removes the requested game from the dictionary of multi-player games
        /// </summary>
        /// <param name="game"> name of game to end </param>
        public void EndGame(string game)
        {
            MultiPlayerGames.Remove(game);
        }

    }

}