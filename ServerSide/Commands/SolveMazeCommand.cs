using MazeLib;
using Newtonsoft.Json.Linq;
using SearchAlgorithmsLib;
using ServerSide;
using System;
using System.IO;
using System.Net.Sockets;

namespace ServerSide
{
    public class SolveMazeCommand : ICommand
    {
        private MazeModel model;

        public SolveMazeCommand(IModel model)
        {
            this.model = model as MazeModel;
        }

        public string Execute(string[] args, TcpClient client = null)
        {
            //Solves an existing maze
            SearchAlgorithmsLib.Solution<Position> solution = model.SolveMaze(args[0], int.Parse(args[1]));
            StepSolution stSol = new StepSolution(args[0], solution);
            stSol.CreateStepSolution();

            Console.WriteLine("Solution for maze: {0}", stSol.Json());
            //Send to client
            NetworkStream n = client.GetStream();
            StreamWriter w = new StreamWriter(n);
            w.WriteLine(stSol.Json());
            w.Flush();
            //Close client
            //client.Close();
            return stSol.ToString();
        }
    }
    public class StepSolution
    {
        private string Name { get; set; }
        private Solution<Position> solution { get; set; }
        private string stepSolution;
        public StepSolution(string name, Solution<Position> solution)
        {
            this.Name = name;
            this.solution = solution;
            stepSolution = "";
        }
        public void CreateStepSolution()
        {
            foreach (State<Position> pos in solution.sol)
            {
                State<Position> p = pos.CameFrom;
                if (p != null)
                {
                    if (p.getState().Col < pos.getState().Col)
                    {
                        stepSolution += "1";
                    }
                    else if (p.getState().Row < pos.getState().Row)
                    {
                        stepSolution += "3";
                    }
                    else if (p.getState().Col > pos.getState().Col)
                    {
                        stepSolution += "2";
                    }
                    else
                    {
                        stepSolution += "0";
                    }
                }
            }
        }
        public string Json()
        {
            JObject sol = new JObject {
                ["Name"] = Name,
                ["Solution"] = stepSolution,
                ["NodesEvaluated"] = solution.getNodes(),
        };
            return sol.ToString();
        }
    }
}