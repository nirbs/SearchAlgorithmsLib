using MazeLib;
using Newtonsoft.Json.Linq;
using SearchAlgorithmsLib;
using ServerSide;
using System;
using System.IO;
using System.Net.Sockets;

namespace ServerSide
{
    /// <summary>
    /// Command to solve a requested maze
    /// </summary>
    public class SolveMazeCommand : ICommand
    {

        /// <summary>
        /// Private Model member
        /// </summary>
        private MazeModel Model;

        /// <summary>
        /// constructor that sets the model of the command
        /// </summary>
        /// <param name="model"></param>
        public SolveMazeCommand(IModel model)
        {
            Model = model as MazeModel;
        }

        /// <summary>
        ///  requests the mazes solution from the model
        /// </summary>
        /// <param name="args"> the name of the maze to be solved </param>
        /// <param name="client"> client that wants the solution </param>
        /// <returns></returns>
        public string Execute(string[] args, TcpClient client = null)
        {
            //Solves an existing maze
            Solution<Position> Solution = Model.SolveMaze(args[0], int.Parse(args[1]));
            //Creates a step by step solution
            StepSolution StepSol = new StepSolution(args[0], Solution);
            StepSol.CreateStepSolution();
            //Send to client
            NetworkStream Stream = client.GetStream();
            StreamWriter Writer = new StreamWriter(Stream);
            Writer.WriteLine(StepSol.Json());
            Writer.Flush();
            return StepSol.ToString();
        }
    }

    /// <summary>
    /// 
    /// </summary>
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