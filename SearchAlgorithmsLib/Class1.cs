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
        public List<State<T>> getSolution() { return solution; }
    }
    public class State<T>
    {
    private T state;    // the state represented by a string
    private double cost;    // cost to reach this state (set by a setter)
    private State<T> cameFrom;    // the state we came from to this state (setter)
    public State(T state)   // CTOR
    {
        this.state= state;
    }
    public bool Equals(State<T> s) // we overload Object's Equals method
    {
        return state.Equals(s.state);
    } 
}
    public interface ISearchable <T>
    {
        State<T> getInitialState();
        State<T> getGoalState();
        List<State<T>> getAllPossibleStates(State<T> s);
    }

    public interface ISearcher<T>
    {
        // the search method
        Solution<T> search (ISearchable<T> searchable);
    // get how many nodes were evaluated by the algorithm
    int getNumberOfNodesEvaluated();
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
    }

}
