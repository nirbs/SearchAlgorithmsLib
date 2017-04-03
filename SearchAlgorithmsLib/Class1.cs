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
        public void addToOpenList(State<T> state, int i)
        {
            openList.Enqueue(state, i);
        }
        public bool contains(State<T> s)
        {
            if (openList.Contains(s))
            {
                return true;
            }
            return false;
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
            endSolution.sol = backtrace;
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
            int x = s.getState().Col;
            int y = s.getState().Row;
            List<State<MazeLib.Position>> listush = new List<State<MazeLib.Position>>();
            if (myMaze[y + 1, x].Equals("Free"))
            {
                MazeLib.Position p = new MazeLib.Position(y + 1, x);

                State<MazeLib.Position> down = new State<MazeLib.Position>(p);
                down.CameFrom = s;
                listush.Add(down);
            }
            if (myMaze[y - 1, x].Equals("Free"))
            {
                MazeLib.Position p = new MazeLib.Position(y - 1, x);
                State<MazeLib.Position> up = new State<MazeLib.Position>(p);
                up.CameFrom = s;
                listush.Add(up);
            }
            if (myMaze[y, x + 1].Equals("Free"))
            {
                MazeLib.Position p = new MazeLib.Position(y, x + 1);
                State<MazeLib.Position> right = new State<MazeLib.Position>(p);
                right.CameFrom = s;
                listush.Add(right);
            }
            if (myMaze[y, x - 1].Equals("Free"))
            {
                MazeLib.Position p = new MazeLib.Position(y, x - 1);
                State<MazeLib.Position> left = new State<MazeLib.Position>(p);
                left.CameFrom = s;
                listush.Add(left);
            }
            return listush;
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
            int i = 0;
            addToOpenList(searchable.getInitialState(), i); // inherited from Searcher
            HashSet<State<T>> closed = new HashSet<State<T>>();
            while (OpenListSize > 0)
            {
                State<T> n = popOpenList(); // inherited from Searcher, removes the best state
                closed.Add(n);
                if (n.Equals(searchable.getGoalState()))
                    return backTrace(n); // private method, back traces through the parents
                                         // calling the delegated method, returns a list of states with n as a parent
                List<State<T>> succerssors = searchable.getAllPossibleStates(n);
                foreach (State<T> s in succerssors)
                {
                    if (!closed.Contains(s) && !contains(s))
                    {
                        // s.setCameFrom(n); // already done by getSuccessors
                        i++;
                        addToOpenList(s, i);
                    }
                    else
                    {
                    }
                }
            }
            return null;
        }
    }
    public class DFS<T> : Searcher<T>
    {
        public override Solution<T> search(ISearchable<T> searchable)
        {
            State<T> current; // = new State<T>();
            Stack<State<T>> beingChecked = new Stack<State<T>>();
            List<State<T>> discovered = new List<State<T>>();
            beingChecked.Push(searchable.getGoalState());

            while(beingChecked.Count != 0)
            {
                current = beingChecked.Pop();
                updateNodesEvaluated();

                if (!discovered.Contains(current))
                {
                    discovered.Add(current);
                    List<State<T>> adjacents = searchable.getAllPossibleStates(current);
                    foreach(State<T> adj in adjacents)
                    {
                        beingChecked.Push(adj);
                    }

                }
            }
        }
        
    }
}
