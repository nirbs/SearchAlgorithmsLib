using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchAlgorithmsLib
{
    public class Solution<T>
    {
        private List<State<T>> solution;
        //public List<State<T>> getSolution() { return solution; }
        public List<State<T>> sol { get; set; }
    }

    public class State<T>
    {
        private T state;    // the state represented by a T
        public double cost { get; set; }    // cost to reach this state (set by a setter)
        //private State<T> cameFrom;    // the state we came from to this state (setter)
        public double Cost { get; set; }
        public State<T> CameFrom { get; set; }
        public State(T state)   // CTOR
        {
            this.state = state;
        }
        public bool Equals(State<T> s) // we overload Object's Equals method
        {
            return state.Equals(s.state);
        }
        public T getState()
        {
            return this.state;
        }

        public static class StatePool
        {
            private static Dictionary<T, State<T>> pool = new Dictionary<T, State<T>>();
            public static State<T> getState(T state)
            {
                if (pool == null || !pool.ContainsKey(state))
                {
                    State<T> newState = new State<T>(state);
                    pool.Add(state, newState);
                }
                
                return pool[state];
            }
            public static bool checkState(State<T> state)
            {
                return pool.ContainsValue(state);
            }

        }

    }
    public interface ISearchable<T>
    {
        State<T> getInitialState();
        State<T> getGoalState();
        List<State<T>> getAllPossibleStates(State<T> s);
    }

    public interface ISearcher<T>
    {
        // the search method
        Solution<T> search(ISearchable<T> searchable);
        // get how many nodes were evaluated by the algorithm
        int getNumberOfNodesEvaluated();
        Solution<T> backTrace(State<T> goal);
    }
    public abstract class Searcher<T> : ISearcher<T>
    {
        private Priority_Queue.SimplePriorityQueue<State<T>> openList;
        private int evaluatedNodes;
        public Searcher()
        {
            openList = new Priority_Queue.SimplePriorityQueue<State<T>>();
            evaluatedNodes = 0;
        }

        public bool found(State<T> s, List<State<T>> list)
        {

            foreach (State<T> t in list)
            {
                if (t.getState().Equals(s.getState()))
                    return true;
            }
            return false;
        }

        protected State<T> popOpenList()
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
        public virtual int getNumberOfNodesEvaluated()
        {
            return evaluatedNodes;
        }
        public abstract Solution<T> search(ISearchable<T> searchable);

        public void addToOpenList(State<T> state)
        {
            openList.Enqueue(state, (float)state.cost);
        }

        public bool contains(State<T> s)
        {
            foreach (State<T> t in openList)
            {
                if (t.getState().Equals(s.getState()))
                {
                    return true;
                }
            }
            /*if (openList.Contains(s))
            {
                return true;
            }*/
            return false;
        }

        public void remove(State<T> s)
        {
            foreach (State<T> t in openList)
            {
                if (t.getState().Equals(s.getState()))
                {
                    openList.Remove(t);
                    return;
                }

            }
        }

        public Solution<T> backTrace(State<T> goal)
        {
            Solution<T> endSolution = new Solution<T>();

            List<State<T>> backtrace = new List<State<T>>();
            Stack<State<T>> backStack = new Stack<State<T>>();
            State<T> camefrom = goal.CameFrom;
            while (camefrom != null)
            {
                backStack.Push(camefrom);
                camefrom = camefrom.CameFrom;

            }
            while (backStack.Count != 0)
            {
                backtrace.Add(backStack.Pop());
            }
            backtrace.Add(goal);
            endSolution.sol = backtrace;
            Console.WriteLine($"PATH SIZE: {backtrace.Count()}");
            /*foreach(State<T> t in endSolution.sol)
            {
                Console.WriteLine(t.getState().ToString());
            }*/

            return endSolution;
        }

        public void updateNodesEvaluated()
        {
            evaluatedNodes++;
        }

        public void initializeEvaluatedNodes()
        {
            evaluatedNodes = 0;
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
        private State<T> goalState;
        private State<T> initialState;
        public Searchable(State<T> initial, State<T> goal)
        {
            initialState = initial;
            goalState = goal;
        }
        public abstract List<State<T>> getAllPossibleStates(State<T> s);

        public State<T> getGoalState()
        {
            return goalState;
        }

        public State<T> getInitialState()
        {
            return initialState;
        }
    }

    public class MazeSearchable : Searchable<MazeLib.Position>
    {
        private MazeLib.Maze myMaze;
        public MazeSearchable(State<MazeLib.Position> initial, State<MazeLib.Position> goal) : base(initial, goal) { }

        public override List<State<MazeLib.Position>> getAllPossibleStates(State<MazeLib.Position> s)
        {
            int x = s.getState().Row;
            int y = s.getState().Col;
            List<State<MazeLib.Position>> adjencyList = new List<State<MazeLib.Position>>();
            if (y + 1 < myMaze.Cols)
            {
                if (myMaze[x, y + 1] == 0)
                {
                    MazeLib.Position p = new MazeLib.Position(x, y + 1);
                    State<MazeLib.Position> right = State<MazeLib.Position>.StatePool.getState(p);
                    if (s.CameFrom==null || (s.CameFrom != null && !s.CameFrom.Equals(right)))
                    {
                        adjencyList.Add(right);
                    }
                    
                }
            }
            if (y - 1 >= 0)
            {
                if (myMaze[x, y - 1] == 0)
                {
                    MazeLib.Position p = new MazeLib.Position(x, y - 1);
                    State<MazeLib.Position> left = State<MazeLib.Position>.StatePool.getState(p);
                    // up.cost = 99999999999;
                    if (s.CameFrom == null || (s.CameFrom != null && !s.CameFrom.Equals(left)))
                    {
                        adjencyList.Add(left);
                    }
                }
            }
            if (x + 1 < myMaze.Rows)
            {
                if (myMaze[x + 1, y] == 0)
                {
                    MazeLib.Position p = new MazeLib.Position(x + 1, y);
                    State<MazeLib.Position> down = State<MazeLib.Position>.StatePool.getState(p);
                    //right.cost = 99999999999;
                    if (s.CameFrom == null || (s.CameFrom!=null && !s.CameFrom.Equals(down)))
                    {
                        adjencyList.Add(down);
                    }
                }
            }
            if (x - 1 >= 0)
            {
                if (myMaze[x - 1, y] == 0)
                {
                    MazeLib.Position p = new MazeLib.Position(x - 1, y);
                    State<MazeLib.Position> up = State<MazeLib.Position>.StatePool.getState(p);
                    // left.cost = 99999999999;
                    if (s.CameFrom == null || (s.CameFrom != null && !s.CameFrom.Equals(up)))
                    {
                        adjencyList.Add(up);
                    }
                }
            }
            return adjencyList;
        }
        public void setMaze(MazeLib.Maze maze)
        {
            myMaze = maze;
        }
    }
    public class BFS<T> : Searcher<T>
    {


        public override Solution<T> search(ISearchable<T> searchable)
        { // Searcher's abstract method overriding
            addToOpenList(searchable.getInitialState()); // inherited from Searcher
            searchable.getInitialState().cost = 0;
            //List<State<T>> closed = new List<State<T>>();
            HashSet<State<T>> closed = new HashSet<State<T>>();

            while (OpenListSize > 0)
            {
                State<T> n = popOpenList(); // inherited from Searcher, removes the best state
                closed.Add(n);
                if (n.getState().Equals(searchable.getGoalState().getState()))
                {
                    Console.WriteLine(n.CameFrom.getState().ToString());
                    return backTrace(n); // private method, back traces through the parents
                                         // calling the delegated method, returns a list of states with n as a parent
                }
                List<State<T>> succerssors = searchable.getAllPossibleStates(n);
                foreach (State<T> s in succerssors)
                {
                    double newCost = n.cost + 1;
                    //First time it is discovered
                    if (!closed.Contains(s) && !contains(s))
                    {
                        s.CameFrom = n;
                        s.cost = newCost;
                        addToOpenList(s);
                    }
                    //If we may want to update its Cost
                    else
                    {
                        //If we want to update cost
                        if (s.cost > newCost)
                        {
                            //If it has not yet been added to discovered list
                            if (contains(s))
                            {
                                remove(s);
                                s.CameFrom = n;
                                s.cost = newCost;
                                addToOpenList(s);
                            }
                            //In open list, but now has a better path
                            /* else
                             {
                                 remove(s);
                                 s.cost = newCost;
                                 s.CameFrom = n;

                             }*/
                        }
                    }
                }
            }
            return null;
        }
        /*{
            double newCost = n.Cost + 1;
            //Not in closed
            if (!closed.Contains(s))
            {
                //Not in closed AND not in open
                if (!contains(s))
                {
                    s.CameFrom = n;
                    s.Cost = newCost;
                    addToOpenList(s);
                }
                //Not in closed but is in open, want to check if we should update priority
                else
                {
                    //The new path is shorter than its previous path
                    if (s.Cost > newCost)
                    {
                        s.Cost = newCost;
                    }
                }
            }


        }
    }
    return null;
}*/
        /*{
            double newCost = n.Cost + 1;
            if (!closed.Contains(s) && !contains(s))
            {
                s.CameFrom = n;
                s.Cost = newCost;
                addToOpenList(s);
            }
            else
            {
                if (s.Cost > newCost)
                {
                    if (!contains(s))
                    {
                        addToOpenList(s);
                    }
                    else
                    {
                        s.Cost = newCost;
                    }
                }
            }
        }
    }
    return null;
}*/
    }




    /*  foreach (State<T> s in succerssors)
      {
          double newCost = n.Cost + 1;
          //Not in closed
          if (!closed.Contains(s))
          {
              //Not in closed AND not in open
              if (!contains(s))
              {
                  s.CameFrom = n;
                  s.Cost = newCost;
                  addToOpenList(s);
              }
              //Not in closed but is in open, want to check if we should update priority
              else
              {
                  //The new path is shorter than its previous path
                  if (s.Cost > newCost)
                  {
                      s.Cost = newCost;
                  }
              }
          }


      }
  }
  return null;
}*/


    public class DFS<T> : Searcher<T>
    {
        public override Solution<T> search(ISearchable<T> searchable)
        {
            State<T> current;
            Stack<State<T>> beingChecked = new Stack<State<T>>();
            List<State<T>> discovered = new List<State<T>>();
            beingChecked.Push(searchable.getInitialState());
            while (beingChecked.Count != 0)
            {
                current = beingChecked.Pop();
                
                updateNodesEvaluated();
                if (current.getState().Equals(searchable.getGoalState().getState()))
                {
                    return backTrace(current);
                }


                if (!discovered.Contains(current))
                {
                    discovered.Add(current);
                    List<State<T>> adjacents = searchable.getAllPossibleStates(current);
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
