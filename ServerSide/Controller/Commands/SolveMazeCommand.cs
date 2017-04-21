using MazeLib;
using Newtonsoft.Json.Linq;
using SearchAlgorithmsLib;
using ServerSide;
using System;
using System.Collections.Generic;
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
        private MazeModel model;
        private Dictionary<string, StepSolution> stepSolutions;

        /// <summary>
        /// constructor that sets the model of the command
        /// </summary>
        /// <param name="model"></param>
        public SolveMazeCommand(IModel model)
        {
            this.model = model as MazeModel;
            stepSolutions = new Dictionary<string, StepSolution>();
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
            Solution<Position> solution = model.SolveMaze(args[0], int.Parse(args[1]));
            //Creates a step by step solution
            StepSolution stepSol;
            if (solution == null)
            {
                stepSol = stepSolutions[args[0]];
            }
            else
            {
                Console.WriteLine("Maze solved");
                stepSol = new StepSolution(args[0], solution);
                stepSol.CreateStepSolution();
                stepSolutions.Add(args[0], stepSol);
            }
            //Send to client
            NetworkStream stream = client.GetStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.WriteLine(stepSol.Json());
            writer.WriteLine("#");
            writer.Flush();
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
        private string name { get; set; }
        /// <summary>
        /// Solution of the maze
        /// </summary>
        private Solution<Position> stepByStepSolution { get; set; }
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
            this.name = name;
            stepByStepSolution = solution;
            stepSolution = "";
        }

        /// <summary>
        /// Method to create the step by step solution
        /// </summary>
        public void CreateStepSolution()
        {
            foreach (State<Position> pos in stepByStepSolution.Sol)
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
            JObject sol = new JObject
            {
                ["Name"] = name,
                ["Solution"] = stepSolution,
                ["NodesEvaluated"] = stepByStepSolution.GetNodes(),
            };
            return sol.ToString();
        }
    }
}