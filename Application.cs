using KingPongServer.Network;

namespace KingPongServer
{
    public class Application
    {
        public static Server ServerInstance;

        static void Main(string[] args)
        {
            Console.WriteLine("Launching KingPong-Server..");
            ServerInstance = new Server();
            ServerInstance.Start();
        }
    }
    
}
