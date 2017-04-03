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

        }

        public void CompareSolvers()
        {
            //create DFSmaze generator

            MazeGeneratorLib.DFSMazeGenerator mazeGenerator = new MazeGeneratorLib.DFSMazeGenerator();
            MazeLib.Maze mazush =  mazeGenerator.Generate(30, 30);
            String s = mazush.ToString();
            int count = 0;
            //prints maze
            for (int i=0; i<mazush.Rows; i++)
            {
                for (int j=0; j < mazush.Cols; j++)
                {
                    Console.Write(s[count]);
                    count++;
                }
                Console.WriteLine();
            }
            //solves in BFS
            SearchAlgorithmsLib.State<MazeLib.Position> entrance = new SearchAlgorithmsLib.State<MazeLib.Position>(mazush.InitialPos);
            SearchAlgorithmsLib.State<MazeLib.Position> exit = new SearchAlgorithmsLib.State<MazeLib.Position>(mazush.GoalPos);
            SearchAlgorithmsLib.MazeSearchable mazeSearchable = new SearchAlgorithmsLib.MazeSearchable(entrance, exit);
            mazeSearchable.setMaze(mazush);
            SearchAlgorithmsLib.BFS<MazeLib.Position> bfs = new SearchAlgorithmsLib.BFS<MazeLib.Position>();
            SearchAlgorithmsLib.Solution<MazeLib.Position> bfsSolution =  bfs.search(mazeSearchable);

            //Solves in DFS
            SearchAlgorithmsLib.DFS<MazeLib.Position> dfs = new SearchAlgorithmsLib.DFS<MazeLib.Position>();
            SearchAlgorithmsLib.Solution<MazeLib.Position> dfsSolution = dfs.search(mazeSearchable);


            //prints how many checks algo did
            Console.WriteLine($"BFS DID {bfs.getNumberOfNodesEvaluated()} evaluations");
            Console.WriteLine($"DFS DID {dfs.getNumberOfNodesEvaluated()} evaluations");

        }
    }
}
