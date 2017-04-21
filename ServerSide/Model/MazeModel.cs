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
        IController controller;

        /// <summary>
        /// Dictionary to contain multi player games
        /// </summary>
        private Dictionary<string, MazeGame> multiPlayerGames;

        /// <summary>
        /// Dictionary to contain single player games
        /// </summary>
        private Dictionary<string, MazeGame> singlePlayerGames;

        /// <summary>
        /// Dictionary with all types of searchers
        /// </summary>
        private Dictionary<int, ISearcher<Position>> searchers;

        /// <summary>
        /// Dictionary of all mazes that have already been solved
        /// </summary>
        private Dictionary<string, Solution<Position>> solvedMazes;


        /// <summary>
        /// Constructor which sets the controller and
        /// initializes all the dictionaries
        /// </summary>
        /// <param name="c"> controller </param>
        public MazeModel(IController c)
        {
            controller = c;

            //Dictionary to contain all mazeGames
            singlePlayerGames = new Dictionary<string, MazeGame>();
            multiPlayerGames = new Dictionary<string, MazeGame>();


            //Contains solutions to the mazes
            solvedMazes = new Dictionary<string, Solution<Position>>();

            searchers = new Dictionary<int, ISearcher<Position>>();

            //Adds searchers to dictionary
            searchers.Add(0, new BFS<Position>());
            searchers.Add(1, new DFS<Position>());
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
            Maze newMaze = new MazeGeneratorLib.DFSMazeGenerator().Generate(row, col);
            newMaze.Name = name;

            //Adds the current client as player1 to this game
            MazeGame newGame = new MazeGame(newMaze);

            if (gameType == "Single")
            {
                Console.WriteLine("Single-Player Game '{0}' Added", name);
                singlePlayerGames.Add(name, newGame);
            }
            else //Multi Player
            {
                Console.WriteLine("Multi-Player Game '{0}' Added", name);
                newGame.AddPlayer(player);
                multiPlayerGames.Add(name, newGame);
                Console.WriteLine(multiPlayerGames[name].Maze.Name);

            }
            return newMaze;
        }


        /// <summary>
        /// Method to return a list of all multi player games
        /// </summary>
        /// <returns> list of names of mazes </returns>
        public string ListAllMazes()
        {
            List<string> KeyList = new List<string>(multiPlayerGames.Keys);
            StringBuilder resultList = new StringBuilder();
            resultList.Append("[\r\n");

            //return a JSON of; all games in dictionary
            foreach (string key in multiPlayerGames.Keys)
            {
                resultList.Append("\"" + key + "\"");
                if (multiPlayerGames[key].IsFull)
                {
                    resultList.Append("-> GAME FULL");
                }
                resultList.Append(",\r\n");
                
            }
            resultList.Append("]");
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
            if (solvedMazes.ContainsKey(name))
            {
                Console.WriteLine("The maze '{0}' has already been solved...Returning solution", name);
                // return solvedMazes[name];
                return null;
            }

            //Selects wanted game from dictionary
            MazeGame game;
            if (singlePlayerGames.Keys.Contains(name))
            {
                game = singlePlayerGames[name];
            } else
            {
                 game = multiPlayerGames[name];
            }
           
            Maze mazeToSolve = game.Maze;

            //Creates a new searchable with start and end point of selected maze
            MazeSearchable searchable = new MazeSearchable(new State<Position>(mazeToSolve.InitialPos), new State<Position>(mazeToSolve.GoalPos));

            //Sets the searchable's maze
            searchable.SetMaze(mazeToSolve);

            //selects the wanted search method
            ISearcher<Position> mazeSearcher = searchers[searchType];

            //solves the maze
            Solution<Position> solution = mazeSearcher.Search(searchable);
            
            //Adds solution to Dictionary
            solvedMazes.Add(name, solution);

            //Convert to JSON and return to client HERE OR IN COMMAND?
            return solution;
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
            if(!multiPlayerGames[game].IsFull)
                multiPlayerGames[game].AddPlayer(player);
            return multiPlayerGames[game];
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
            if(singlePlayerGames.Keys.Contains(name))
            {
                return singlePlayerGames[name];
            } else
            {
                return multiPlayerGames[name];
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
            foreach (MazeGame v in multiPlayerGames.Values)
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
            foreach(MazeGame Game in multiPlayerGames.Values)
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
            multiPlayerGames.Remove(game);
        }

    }

}