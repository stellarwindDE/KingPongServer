using KingPongServer.Network;

namespace KingPongServer
{
    public class Player
    {
        public PlayerThread playerThread;
        private Game game;

        public int score { get; set; }
        
        public Player(PlayerThread playerThread, Game game)
        {
            this.playerThread = playerThread;
            this.score = 0;
            this.game = game;
        }
    }
}
