using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectAdapter
{
    class Program
    {
        static void Main(string[] args)
        {
            Program p = new Program();
            p.CompareSolvers();
            Console.Read();
        }

        public void CompareSolvers()
        {
            //create DFSmaze generator

            MazeGeneratorLib.DFSMazeGenerator mazeGenerator = new MazeGeneratorLib.DFSMazeGenerator();
            MazeLib.Maze mazush =  mazeGenerator.Generate(200,200);
            string s = mazush.ToString();

            //prints maze
            Console.WriteLine(s);
            Console.WriteLine($"START: {mazush.InitialPos}");
            Console.WriteLine($"END: {mazush.GoalPos}");

            //solves in BFS
            SearchAlgorithmsLib.State<MazeLib.Position> entrance = new SearchAlgorithmsLib.State<MazeLib.Position>(mazush.InitialPos);
            SearchAlgorithmsLib.State<MazeLib.Position> exit = new SearchAlgorithmsLib.State<MazeLib.Position>(mazush.GoalPos);
            SearchAlgorithmsLib.MazeSearchable mazeSearchable = new SearchAlgorithmsLib.MazeSearchable(entrance, exit);
            mazeSearchable.SetMaze(mazush);
            SearchAlgorithmsLib.BFS<MazeLib.Position> bfs = new SearchAlgorithmsLib.BFS<MazeLib.Position>();
            SearchAlgorithmsLib.Solution<MazeLib.Position> bfsSolution =  bfs.Search(mazeSearchable);

            //Solves in DFS
            

            //prints how many checks algo did
            Console.WriteLine($"BFS DID {bfs.GetNumberOfNodesEvaluated()} evaluations");

            SearchAlgorithmsLib.DFS<MazeLib.Position> dfs = new SearchAlgorithmsLib.DFS<MazeLib.Position>();
            SearchAlgorithmsLib.Solution<MazeLib.Position> dfsSolution = dfs.Search(mazeSearchable);

            Console.WriteLine($"DFS DID {dfs.GetNumberOfNodesEvaluated()} evaluations");

        }
    }
}
