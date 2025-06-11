using KingPongServer.Network;

namespace KingPongServer
{
    public class Player
    {
        public PlayerThread playerThread;
        private Game game;
        private int id;

        public int score { get; set; }
        
        public Player(PlayerThread playerThread, Game game, int id)
        {
            this.playerThread = playerThread;
            this.score = 0;
            this.game = game;
            this.id = id;
        }
    }
}
