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
            Writer.WriteLine("#");

            Writer.Flush();
            return "CLOSE";
        }
    }

    /// <summary>
    /// class to create a step by step solution
    /// </summary>
    public class StepSolution
    {
        /// <summary>
        /// Name of maze solution
        /// </summary>
        private string Name { get; set; }
        /// <summary>
        /// Solution of the maze
        /// </summary>
        private Solution<Position> StepByStepSolution { get; set; }
        /// <summary>
        /// string of solution itself
        /// </summary>
        private string stepSolution;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"> name of solution </param>
        /// <param name="solution"> solution itself </param>
        public StepSolution(string name, Solution<Position> solution)
        {
            Name = name;
            StepByStepSolution = solution;
            stepSolution = "";
        }

        /// <summary>
        /// Method to create the step by step solution
        /// </summary>
        public void CreateStepSolution()
        {
            foreach (State<Position> pos in StepByStepSolution.Sol)
            {
                State<Position> p = pos.CameFrom;
                if (p != null)
                {
                    if (p.GetState().Col < pos.GetState().Col)
                    {
                        stepSolution += "1";
                    }
                    else if (p.GetState().Row < pos.GetState().Row)
                    {
                        stepSolution += "3";
                    }
                    else if (p.GetState().Col > pos.GetState().Col)
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

        /// <summary>
        /// Method to turn the solution into JSON form
        /// </summary>
        /// <returns> the string of the solution, JSON form</returns>
        public string Json()
        {
            JObject sol = new JObject {
                ["Name"] = Name,
                ["Solution"] = stepSolution,
                ["NodesEvaluated"] = StepByStepSolution.GetNodes(),
        };
            return sol.ToString();
        }
    }
}