using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchAlgorithmsLib
{
    /// <summary>
    /// Generic class to hold a list of solutions for a given problem
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Solution<T>
    {
        private List<State<T>> mySolution;
        public List<State<T>> Sol { get; set; }
        private int nodesEvaluated = 0;
        public int GetNodes()
        {
            return nodesEvaluated;
        }
        public void SetNodes(int num)
        {
            nodesEvaluated = num;
        }
    }
    /// <summary>
    /// Generic class the presents a state in a given problem
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class State<T>
    {
        private T myState;    // the state represented by a T
        public double Cost { get; set; }    // cost to reach this state (set by a setter) 
        public State<T> CameFrom { get; set; }  // the state we came from to this state (setter)
        public State(T state)   // CTOR
        {
            this.myState = state;
        }
        public bool Equals(State<T> s) // we overload Object's Equals method
        {
            return myState.Equals(s.myState);
        }
        public T GetState()
        {
            return this.myState;
        }

        /// <summary>
        /// A pool of states that creats only new stats and return reference to existed ones
        /// </summary>
        public static class StatePool
        {
            private static Dictionary<T, State<T>> pool = new Dictionary<T, State<T>>();
            public static State<T> GetState(T state)
            {
                if (pool == null || !pool.ContainsKey(state))
                {
                    State<T> NewState = new State<T>(state);
                    pool.Add(state, NewState);
                }

                return pool[state];
            }

            /// <summary>
            /// Method to check whether a state is existed or not
            /// </summary>
            /// <param name="state"></param>
            /// <returns></returns>
            public static bool CheckState(State<T> state)
            {
                return pool.ContainsValue(state);
            }

        }
    }

    /// <summary>
    /// Generic interface for searchable problems
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISearchable<T>
    {
        State<T> GetInitialState();
        State<T> GetGoalState();
        List<State<T>> GetAllPossibleStates(State<T> s);
    }

    /// <summary>
    /// Generic interface for searchers
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISearcher<T>
    {
        // the search method
        Solution<T> Search(ISearchable<T> searchable);
        // get how many nodes were evaluated by the algorithm
        int GetNumberOfNodesEvaluated();
        Solution<T> BackTrace(State<T> goal);
    }

    /// <summary>
    /// Generic abstract class for searchers
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Searcher<T> : ISearcher<T>
    {
        private Priority_Queue.SimplePriorityQueue<State<T>> openList;
        private int evaluatedNodes;
        public Searcher()
        {
            openList = new Priority_Queue.SimplePriorityQueue<State<T>>();
            evaluatedNodes = 0;
        }

        /// <summary>
        /// Method to check whether a given state is the list a of discovered states
        /// </summary>
        /// <param name="s"></param>
        /// <param name="list"></param>
        /// <returns>true if found and false otherwise</returns>
        public bool Found(State<T> s, List<State<T>> list)
        {

            foreach (State<T> t in list)
            {
                if (t.GetState().Equals(s.GetState()))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Method to raise number of evaluated nodes and pop the next state from the list
        /// </summary>
        /// <returns>The next state</returns>
        protected State<T> PopOpenList()
        {
            evaluatedNodes++;
            return openList.Dequeue();
        }
        // a property of openList
        public int OpenListSize
        { // it is a read-only property
            get { return openList.Count; }
        }

        // ISearcher’smethods:
        public virtual int GetNumberOfNodesEvaluated()
        {
            return evaluatedNodes;
        }
        public abstract Solution<T> Search(ISearchable<T> searchable);

        /// <summary>
        /// Method to add new state to the discovered list
        /// </summary>
        /// <param name="state"></param>
        public void AddToOpenList(State<T> state)
        {
            openList.Enqueue(state, (float)state.Cost);
        }

        /// <summary>
        /// Method to check whether a given state is in the discovered list
        /// </summary>
        /// <param name="s"></param>
        /// <returns>true if the state is in the list and false otherwise</returns>
        public bool Contains(State<T> s)
        {
            foreach (State<T> t in openList)
            {
                if (t.GetState().Equals(s.GetState()))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Method to remove a given state from the discovered list
        /// </summary>
        /// <param name="s"></param>
        public void Remove(State<T> s)
        {
            foreach (State<T> t in openList)
            {
                if (t.GetState().Equals(s.GetState()))
                {
                    openList.Remove(t);
                    return;
                }

            }
        }

        /// <summary>
        /// Method that start from goal state and creates a solution that contains all states to the initial state
        /// </summary>
        /// <param name="goal"></param>
        /// <returns>solution of states</returns>
        public Solution<T> BackTrace(State<T> goal)
        {
            Solution<T> endSolution = new Solution<T>();

            List<State<T>> backtrace = new List<State<T>>();
            Stack<State<T>> backStack = new Stack<State<T>>();
            State<T> CameFrom = goal.CameFrom;
            while (CameFrom != null)
            {
                backStack.Push(CameFrom);
                CameFrom = CameFrom.CameFrom;

            }
            while (backStack.Count != 0)
            {
                backtrace.Add(backStack.Pop());
            }
            backtrace.Add(goal);
            endSolution.Sol = backtrace;
            endSolution.SetNodes(evaluatedNodes);
            return endSolution;
        }

        /// <summary>
        /// Method to raise number of evaluated nodes
        /// </summary>
        public void UpdateNodesEvaluated()
        {
            evaluatedNodes++;
        }

        /// <summary>
        /// Method to initiliaze evaluated nodes to 0
        /// </summary>
        public void InitializeEvaluatedNodes()
        {
            evaluatedNodes = 0;
        }
    }

    /// <summary>
    /// Comperable of cost for priority queue
    /// </summary>
    public class CostComparable : IComparable<CostComparable>
    {
        public int Cost { get; set; }
        public int CompareTo(CostComparable otherCost)
        {
            return this.Cost.CompareTo(otherCost.Cost);
        }
    }

    /// <summary>
    /// Abstarct class for searchable objects
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Searchable<T> : ISearchable<T>
    {
        private State<T> GoalState;
        private State<T> InitialState;
        public Searchable(State<T> initial, State<T> goal) //Ctor
        {
            InitialState = initial;
            GoalState = goal;
        }
        /// <summary>
        /// Abstract method to create a list of neighbors of a given state
        /// </summary>
        /// <param name="s"></param>
        /// <returns>list of states</returns>
        public abstract List<State<T>> GetAllPossibleStates(State<T> s);

        public State<T> GetGoalState() //Get method for goal state
        {
            return GoalState;
        }

        public State<T> GetInitialState() //Get method for initial state
        {
            return InitialState;
        }
    }

    /// <summary>
    /// Specific searchable class for mazes
    /// </summary>
    public class MazeSearchable : Searchable<MazeLib.Position>
    {
        private MazeLib.Maze MyMaze;
        public MazeSearchable(State<MazeLib.Position> initial, State<MazeLib.Position> goal) : base(initial, goal) { }

        /// <summary>
        /// Method to create a list of neighbors of a given position state
        /// </summary>
        /// <param name="s"></param>
        /// <returns>list of neighbors</returns>
        public override List<State<MazeLib.Position>> GetAllPossibleStates(State<MazeLib.Position> s)
        {
            int X = s.GetState().Row;
            int Y = s.GetState().Col;
            List<State<MazeLib.Position>> adjencyList = new List<State<MazeLib.Position>>();
            if (Y + 1 < MyMaze.Cols)
            {
                if (MyMaze[X, Y + 1] == 0)
                {
                    MazeLib.Position p = new MazeLib.Position(X, Y + 1);
                    State<MazeLib.Position> right = State<MazeLib.Position>.StatePool.GetState(p);
                    if (s.CameFrom == null || (s.CameFrom != null && !s.CameFrom.Equals(right)))
                    {
                        adjencyList.Add(right);
                    }

                }
            }
            if (Y - 1 >= 0)
            {
                if (MyMaze[X, Y - 1] == 0)
                {
                    MazeLib.Position p = new MazeLib.Position(X, Y - 1);
                    State<MazeLib.Position> left = State<MazeLib.Position>.StatePool.GetState(p);
                    if (s.CameFrom == null || (s.CameFrom != null && !s.CameFrom.Equals(left)))
                    {
                        adjencyList.Add(left);
                    }
                }
            }
            if (X + 1 < MyMaze.Rows)
            {
                if (MyMaze[X + 1, Y] == 0)
                {
                    MazeLib.Position p = new MazeLib.Position(X + 1, Y);
                    State<MazeLib.Position> down = State<MazeLib.Position>.StatePool.GetState(p);
                    if (s.CameFrom == null || (s.CameFrom != null && !s.CameFrom.Equals(down)))
                    {
                        adjencyList.Add(down);
                    }
                }
            }
            if (X - 1 >= 0)
            {
                if (MyMaze[X - 1, Y] == 0)
                {
                    MazeLib.Position p = new MazeLib.Position(X - 1, Y);
                    State<MazeLib.Position> up = State<MazeLib.Position>.StatePool.GetState(p);
                    if (s.CameFrom == null || (s.CameFrom != null && !s.CameFrom.Equals(up)))
                    {
                        adjencyList.Add(up);
                    }
                }
            }
            return adjencyList;
        }
        public void SetMaze(MazeLib.Maze maze) //Set method for maze
        {
            MyMaze = maze;
        }
    }

    /// <summary>
    /// Specific searcher class of BFS type 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BFS<T> : Searcher<T>
    {
        public override Solution<T> Search(ISearchable<T> searchable)
        { // Searcher's abstract method overriding
            AddToOpenList(searchable.GetInitialState()); // inherited from Searcher
            searchable.GetInitialState().Cost = 0;
            HashSet<State<T>> closed = new HashSet<State<T>>();

            while (OpenListSize > 0)
            {
                State<T> N = PopOpenList(); // inherited from Searcher, removes the best state
                closed.Add(N);

                if (N.GetState().Equals(searchable.GetGoalState().GetState()))
                {
                    Console.WriteLine(N.CameFrom.GetState().ToString());
                    return BackTrace(N); // private method, back traces through the parents
                                         // calling the delegated method, returns a list of states with n as a parent
                }
                List<State<T>> succerssors = searchable.GetAllPossibleStates(N);
                foreach (State<T> s in succerssors)
                {
                    double newCost = N.Cost + 1;
                    //First time it is discovered
                    if (!closed.Contains(s) && !Contains(s))
                    {
                        s.CameFrom = N;
                        s.Cost = newCost;
                        AddToOpenList(s);
                    }
                    //If we may want to update its Cost
                    else
                    {
                        //If we want to update cost
                        if (s.Cost > newCost)
                        {
                            //If it has been added to discovered list
                            if (Contains(s))
                            {
                                Remove(s);
                                s.CameFrom = N;
                                s.Cost = newCost;
                                AddToOpenList(s);
                            }
                        }
                    }
                }
            }
            return null;
        }
    }

    /// <summary>
    /// Specific searcher class of DFS type 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DFS<T> : Searcher<T>
    {
        public override Solution<T> Search(ISearchable<T> searchable)
        {
            State<T> current;
            Stack<State<T>> beingChecked = new Stack<State<T>>();
            List<State<T>> discovered = new List<State<T>>();
            beingChecked.Push(searchable.GetInitialState());
            while (beingChecked.Count != 0)
            {
                current = beingChecked.Pop();
                UpdateNodesEvaluated();
                if (current.GetState().Equals(searchable.GetGoalState().GetState()))
                {
                    return BackTrace(current);
                }
                if (!discovered.Contains(current))
                {
                    discovered.Add(current);
                    List<State<T>> adjacents = searchable.GetAllPossibleStates(current);
                    foreach (State<T> adj in adjacents)
                    {
                        adj.CameFrom = current;
                        beingChecked.Push(adj);
                    }
                }
            }
            return null;
        }
    }
}
