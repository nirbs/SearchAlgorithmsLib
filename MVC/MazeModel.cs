using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SearchAlgorithmsLib;
using MazeLib;

namespace MVC { 


    public class MazeModel : IModel
    {
        //String is name of game, key is mazeSearchable itself
       
        private Dictionary<string, MazeSearchable> mazeGames;
        //private Searcher<Position> searcher;
        
        public MazeModel()
        {
            mazeGames = new Dictionary<string, MazeSearchable>();
        }

        public void addMazeGame(int row, int col)
        {
            //return new MazeGeneratorLib.DFSMazeGenerator().Generate(row, col);
        }

    }

}

