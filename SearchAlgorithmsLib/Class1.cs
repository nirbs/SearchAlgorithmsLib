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
        private double cost;    // cost to reach this state (set by a setter)
        private State<T> cameFrom;    // the state we came from to this state (setter)
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
            openList.Enqueue(state, (float)state.Cost);
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
            foreach(State<T> t in endSolution.sol)
            {
                Console.WriteLine(t.getState().ToString());
            }
               
            return endSolution;
        }

        public void updateNodesEvaluated()
        {
            evaluatedNodes++;
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
                    State<MazeLib.Position> down = new State<MazeLib.Position>(p);
                    down.Cost = 1000;
                    adjencyList.Add(down);
                }
            }
            if (y - 1 >= 0)
            {
                if (myMaze[x, y - 1] == 0)
                {
                    MazeLib.Position p = new MazeLib.Position(x, y - 1);
                    State<MazeLib.Position> up = new State<MazeLib.Position>(p);
                    up.Cost = 1000;

                    adjencyList.Add(up);
                }
            }
            if (x + 1 < myMaze.Rows)
            {
                if (myMaze[x + 1, y] == 0)
                {
                    MazeLib.Position p = new MazeLib.Position(x + 1,y);
                    State<MazeLib.Position> right = new State<MazeLib.Position>(p);
                    right.Cost = 1000;
                    adjencyList.Add(right);
                }
            }
            if (x - 1 >= 0)
            {
                if (myMaze[x - 1, y] == 0)
                {
                    MazeLib.Position p = new MazeLib.Position(x - 1, y);
                    State<MazeLib.Position> left = new State<MazeLib.Position>(p);
                    left.Cost = 1000;
                    adjencyList.Add(left);
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
            searchable.getInitialState().Cost = 0;
            List<State<T>> closed = new List<State<T>>();



            while (OpenListSize > 0)
            {
                State<T> n = popOpenList(); // inherited from Searcher, removes the best state
                closed.Add(n);
                if (n.getState().Equals(searchable.getGoalState().getState()))
                {
                    Console.WriteLine(n.CameFrom.getState().ToString());
                    return backTrace(n); // private method, back traces through the parents
                                         // calling the delegated method, returns a list of states with n as a parent
                }  List<State<T>> succerssors = searchable.getAllPossibleStates(n);
                foreach (State<T> s in succerssors)
                {
                    double newCost = n.Cost + 1;
                    //First time it is discovered
                    if (!found(s, closed) && !contains(s))
                    {
                        s.CameFrom = n;
                        s.Cost = newCost;
                        addToOpenList(s);
                    }
                    //If we may want to update its Cost
                    else
                    {
                        //If we want to update cost
                        if (s.Cost > newCost)
                        {
                            //If it has not yet been added to discovered list
                            if (!contains(s))
                            {
                                s.CameFrom = n;
                                s.Cost = newCost;
                                addToOpenList(s);
                            }
                            //In open list, but now has a better path
                            else
                            {
                                remove(s);
                                s.Cost = newCost;
                                s.CameFrom = n;
                                addToOpenList(s);
                            }
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


                    if (!found(current, discovered))
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
    