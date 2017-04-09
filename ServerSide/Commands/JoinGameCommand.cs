using ServerSide;
using System;
using System.Net.Sockets;

namespace MVC
{
    public class JoinGameCommand : ICommand
    {
        private MazeModel model;

        public JoinGameCommand(IModel model)
        {
            this.model = model as MazeModel;
        }

        public string Execute(string[] args, TcpClient client = null)
        {
            //args[0] is name of game to join
            
            //Returns the game that this player joined
            MazeGame game = model.AddPlayer(args[0], client);
            
            //Send the maze info to player 1 and 2 using 'game' above    
            return "YES";
        }
    }
}