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
        private List<State<T>> MySolution;
        public List<State<T>> Sol { get; set; }
        private int NodesEvaluated=0;
        public int GetNodes()
        {
            return NodesEvaluated;
        }
        public void SetNodes (int num)
        {
            NodesEvaluated = num;
        }
    }
    /// <summary>
    /// Generic class the presents a state in a given problem
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class State<T>
    {
        private T MyState;    // the state represented by a T
        public double Cost { get; set; }    // cost to reach this state (set by a setter) 
        public State<T> CameFrom { get; set; }  // the state we came from to this state (setter)
        public State(T state)   // CTOR
        {
            this.MyState = state;
        }
        public bool Equals(State<T> s) // we overload Object's Equals method
        {
            return MyState.Equals(s.MyState);
        }
        public T GetState()
        {
            return this.MyState;
        }

        /// <summary>
        /// A pool of states that creats only new stats and return reference to existed ones
        /// </summary>
        public static class StatePool
        {
            private static Dictionary<T, State<T>> Pool = new Dictionary<T, State<T>>();
            public static State<T> GetState(T state)
            {
                if (Pool == null || !Pool.ContainsKey(state))
                {
                    State<T> NewState = new State<T>(state);
                    Pool.Add(state, NewState);
                }
                
                return Pool[state];
            }
            /// <summary>
            /// Method to check whether a state is existed or not
            /// </summary>
            /// <param name="state"></param>
            /// <returns></returns>
            public static bool CheckState(State<T> state)
            {
                return Pool.ContainsValue(state);
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
        private Priority_Queue.SimplePriorityQueue<State<T>> OpenList;
        private int EvaluatedNodes;
        public Searcher()
        {
            OpenList = new Priority_Queue.SimplePriorityQueue<State<T>>();
            EvaluatedNodes = 0;
        }

        public bool Found(State<T> s, List<State<T>> list)
        {

            foreach (State<T> t in list)
            {
                if (t.GetState().Equals(s.GetState()))
                    return true;
            }
            return false;
        }

        protected State<T> PopOpenList()
        {
            EvaluatedNodes++;
            return OpenList.Dequeue();
        }
        // a property of openList
        public int OpenListSize
        { // it is a read-only property
            get { return OpenList.Count; }
        }

        // ISearcher’smethods:
        public virtual int GetNumberOfNodesEvaluated()
        {
            return EvaluatedNodes;
        }
        public abstract Solution<T> Search(ISearchable<T> searchable);

        public void AddToOpenList(State<T> state)
        {
            OpenList.Enqueue(state, (float)state.Cost);
        }

        public bool Contains(State<T> s)
        {
            foreach (State<T> t in OpenList)
            {
                if (t.GetState().Equals(s.GetState()))
                {
                    return true;
                }
            }
            return false;
        }

        public void Remove(State<T> s)
        {
            foreach (State<T> t in OpenList)
            {
                if (t.GetState().Equals(s.GetState()))
                {
                    OpenList.Remove(t);
                    return;
                }

            }
        }

        public Solution<T> BackTrace(State<T> goal)
        {
            Solution<T> EndSolution = new Solution<T>();

            List<State<T>> Backtrace = new List<State<T>>();
            Stack<State<T>> BackStack = new Stack<State<T>>();
            State<T> CameFrom = goal.CameFrom;
            while (CameFrom != null)
            {
                BackStack.Push(CameFrom);
                CameFrom = CameFrom.CameFrom;

            }
            while (BackStack.Count != 0)
            {
                Backtrace.Add(BackStack.Pop());
            }
            Backtrace.Add(goal);
            EndSolution.Sol = Backtrace;
            Console.WriteLine($"PATH SIZE: {Backtrace.Count()}");
            EndSolution.SetNodes(EvaluatedNodes);
       
            return EndSolution;
        }

        public void UpdateNodesEvaluated()
        {
            EvaluatedNodes++;
        }

        public void InitializeEvaluatedNodes()
        {
            EvaluatedNodes = 0;
        }
    }

    public class CostComparable: IComparable<CostComparable>
    {
        public int Cost { get; set; }
        public int CompareTo(CostComparable otherCost)
        {
            return  this.Cost.CompareTo(otherCost.Cost);
        }
    }

    public abstract class Searchable<T> : ISearchable<T>
    {
        private State<T> GoalState;
        private State<T> InitialState;
        public Searchable(State<T> initial, State<T> goal)
        {
            InitialState = initial;
            GoalState = goal;
        }
        public abstract List<State<T>> GetAllPossibleStates(State<T> s);

        public State<T> GetGoalState()
        {
            return GoalState;
        }

        public State<T> GetInitialState()
        {
            return InitialState;
        }
    }

    public class MazeSearchable : Searchable<MazeLib.Position>
    {
        private MazeLib.Maze MyMaze;
        public MazeSearchable(State<MazeLib.Position> initial, State<MazeLib.Position> goal) : base(initial, goal) { }

        public override List<State<MazeLib.Position>> GetAllPossibleStates(State<MazeLib.Position> s)
        {
            int X = s.GetState().Row;
            int Y = s.GetState().Col;
            List<State<MazeLib.Position>> AdjencyList = new List<State<MazeLib.Position>>();
            if (Y + 1 < MyMaze.Cols)
            {
                if (MyMaze[X, Y + 1] == 0)
                {
                    MazeLib.Position P = new MazeLib.Position(X, Y + 1);
                    State<MazeLib.Position> Right = State<MazeLib.Position>.StatePool.GetState(P);
                    if (s.CameFrom==null || (s.CameFrom != null && !s.CameFrom.Equals(Right)))
                    {
                        AdjencyList.Add(Right);
                    }
                    
                }
            }
            if (Y - 1 >= 0)
            {
                if (MyMaze[X, Y - 1] == 0)
                {
                    MazeLib.Position P = new MazeLib.Position(X, Y - 1);
                    State<MazeLib.Position> Left = State<MazeLib.Position>.StatePool.GetState(P);
                    if (s.CameFrom == null || (s.CameFrom != null && !s.CameFrom.Equals(Left)))
                    {
                        AdjencyList.Add(Left);
                    }
                }
            }
            if (X + 1 < MyMaze.Rows)
            {
                if (MyMaze[X + 1, Y] == 0)
                {
                    MazeLib.Position P = new MazeLib.Position(X + 1, Y);
                    State<MazeLib.Position> down = State<MazeLib.Position>.StatePool.GetState(P);
                    if (s.CameFrom == null || (s.CameFrom!=null && !s.CameFrom.Equals(down)))
                    {
                        AdjencyList.Add(down);
                    }
                }
            }
            if (X - 1 >= 0)
            {
                if (MyMaze[X - 1, Y] == 0)
                {
                    MazeLib.Position P = new MazeLib.Position(X - 1, Y);
                    State<MazeLib.Position> up = State<MazeLib.Position>.StatePool.GetState(P);
                    if (s.CameFrom == null || (s.CameFrom != null && !s.CameFrom.Equals(up)))
                    {
                        AdjencyList.Add(up);
                    }
                }
            }
            return AdjencyList;
        }
        public void SetMaze(MazeLib.Maze maze)
        {
            MyMaze = maze;
        }
    }
    public class BFS<T> : Searcher<T>
    {


        public override Solution<T> Search(ISearchable<T> searchable)
        { // Searcher's abstract method overriding
            AddToOpenList(searchable.GetInitialState()); // inherited from Searcher
            searchable.GetInitialState().Cost = 0;
            HashSet<State<T>> Closed = new HashSet<State<T>>();

            while (OpenListSize > 0)
            {
                State<T> N = PopOpenList(); // inherited from Searcher, removes the best state
                Closed.Add(N);

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
                    if (!Closed.Contains(s) && !Contains(s))
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

    public class DFS<T> : Searcher<T>
    {
        public override Solution<T> Search(ISearchable<T> searchable)
        {
            State<T> Current;
            Stack<State<T>> BeingChecked = new Stack<State<T>>();
            List<State<T>> Discovered = new List<State<T>>();
            BeingChecked.Push(searchable.GetInitialState());
            while (BeingChecked.Count != 0)
            {
                Current = BeingChecked.Pop();
                
                UpdateNodesEvaluated();
                if (Current.GetState().Equals(searchable.GetGoalState().GetState()))
                {
                    return BackTrace(Current);
                }


                if (!Discovered.Contains(Current))
                {
                    Discovered.Add(Current);
                    List<State<T>> adjacents = searchable.GetAllPossibleStates(Current);
                    foreach (State<T> adj in adjacents)
                    {
                        adj.CameFrom = Current;
                        BeingChecked.Push(adj);
                    }

                }

            }
            return null;
        }

    }
}
