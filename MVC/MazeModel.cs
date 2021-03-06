﻿using System;
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
        private Dictionary<int, ISearcher<Position>> searchers;
        private Dictionary<string, Solution<Position>> solvedMazes;
        
        public MazeModel()
        {
            //Dictionary to contain all mazeGames
            mazeGames = new Dictionary<string, MazeGame>();

            //Contains solutions to the mazes
            solvedMazes = new Dictionary<string, Solution<Position>>();

            searchers = new Dictionary<int, ISearcher<Position>>();
            //Adds searchers to dictionary
            searchers.Add(0, new BFS<Position>());
            searchers.Add(1, new DFS<Position>());
        }

        public Maze GenerateMaze(string name, int row, int col, TcpClient client)
        {
            //Creates the maze
            Maze newMaze = new MazeGeneratorLib.DFSMazeGenerator().Generate(row, col);
            
            //Adds the current client as player1 to this game
            MazeGame newGame = new MazeGame(newMaze, client);

            //Adds game to dictionary
            mazeGames.Add(name, newGame);
            return newMaze;
        }

        public string ListAllMazes()
        {
            //return a JSON of all games in dictionary
            return "YES";
        }

        public Solution<Position> SolveMaze(string name, int searchType)
        {
            //Already solved this maze
            if(solvedMazes.ContainsKey(name))
            {
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
            mazeGames[game].AddPlayer(player);
            return mazeGames[game];
        }
        public Maze GenerateMaze(int row, int col)
        {
            return new MazeGeneratorLib.DFSMazeGenerator().Generate(row, col);

        }

        public Maze GetMaze(string name)
        {
            return mazeGames[name].maze;
        }

        public TcpClient GetOpponent(TcpClient player)
        {
            return new TcpClient();
            //Find correct game and send back other player
        }

    }

}

