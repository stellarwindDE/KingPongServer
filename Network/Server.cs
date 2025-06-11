using System.Net;
using System.Net.Sockets;
using System.Text;

namespace KingPongServer.Network
{
    public class Server
    {
        private const string KEY_PHRASE = "better atom task thank dynamic audit mixture onion fog";
        private const int PORT = 80;

        private Player[] players = new Player[2];
        private int playerCount = 0;
        private TcpListener listener;


        public Game gameInstance;

        public Server()
        {
            gameInstance = new Game();
            listener = new TcpListener(IPAddress.Any, PORT);
            listener.Start();
            Console.WriteLine($"Listening for new connections on 0.0.0.0:{PORT}..");
        }

        public void Start()
        {
            byte[] keyPhraseBytes = Encoding.UTF8.GetBytes(KEY_PHRASE);
            byte[] buffer = new byte[keyPhraseBytes.Length];

            while (true)
            {
                Console.WriteLine($"Waiting for client connection...(playerCount: {playerCount})");
                TcpClient client = listener.AcceptTcpClient();

                try
                {
                    client.GetStream().Read(buffer, 0, buffer.Length);

                    if (buffer.SequenceEqual(keyPhraseBytes))    // Die eingehende Verbindung stammt tatsächlich von unserer Anwendung
                    {
                        Console.WriteLine($"Client connected: {client.Client.RemoteEndPoint}");
                        playerCount++;

                        PlayerThread playerThread = new PlayerThread(client);
                        playerThread.Start();
                        playerThread.QueuePacket(new GameStatePacket(GameState.WAITING, playerCount));

                        players[findFirstAvailablePlayerIndex()] = new Player(playerThread, gameInstance, playerCount);
                        
                        if(playerCount == 2) 
                        {
                            gameInstance.StartGame(players[0], players[1]);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Invalid connection attempt from: {client.Client.RemoteEndPoint}");
                        client.Close();
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Error accepting client: {ex.Message}");
                }
                
            }
        }

        public void handlePlayerDisconnect(PlayerThread playerThread)
        {
            Console.WriteLine($"Player disconnected: {playerThread.getRemoteEndPoint()}");
            for(int i = 0; i < players.Length; i++)
            {
                if(players[i].playerThread == playerThread)
                {
                    players[i] = null;
                    playerCount--;
                    break;
                }
            }
        }

        public int findFirstAvailablePlayerIndex()
        {
            for(int i = 0; i < players.Length; i++)
            {
                if(players[i] == null) return i;
            }
            return -1;
        }

    }
}
