using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using MessagePack;
using System.Net.Security;

namespace KingPongServer.Network
{
    public class PlayerThread
    {   
        private TcpClient client;
        private Thread thread;
        private ConcurrentQueue<IPacket> packetQueue;
        private bool isRunning;

        public Player player;

        public PlayerThread(TcpClient client)
        {
            this.client = client;
            this.thread = new Thread(Run);
            this.packetQueue = new ConcurrentQueue<IPacket>();
            this.isRunning = true;
        }

        public void Start()
        {
            thread.Start();
        }

        public void QueuePacket(IPacket packet)
        {
            if (isRunning)
            {
                packetQueue.Enqueue(packet);
            }
        }


        public void Stop()
        {
            isRunning = false;
            try
            {
                client.Close();
            }
            catch { }
        }

        private void Run()
        {
            try
            {
                NetworkStream stream = client.GetStream();
                
                while (isRunning)
                {
                    // eingehende pakete empfangen
                    if (stream.DataAvailable)
                    {
                        Console.WriteLine("Data available");
                        try
                        {
                            // First read the 2-byte length prefix
                            byte[] lengthBuffer = new byte[2];
                            int lengthBytesRead = stream.Read(lengthBuffer, 0, 2);
                            
                            if (lengthBytesRead == 0)
                            {
                                // Verbindung von Client geschlossen
                                break;
                            }

                            // Convert the 2 bytes to packet length
                            int packetLength = BitConverter.ToInt16(lengthBuffer, 0);
                            System.Console.WriteLine($"Packet length: {packetLength}");
                            
                            // Read the actual packet data
                            byte[] buffer = new byte[packetLength];
                            int bytesRead = stream.Read(buffer, 0, packetLength);
                            
                            if (bytesRead == 0)
                            {
                                // Verbindung von Client geschlossen
                                break;
                            }
                            System.Console.WriteLine($"Bytes read: {buffer[0]}, {buffer[1]}");

                            IPacket message = MessagePackSerializer.Deserialize<IPacket>(buffer);
                            // TODO: eingehende pakete verarbeiten
                            handlePacket(message);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error reading from client: {ex.Message}");
                            break;
                        }
                    }

                    // ausgehende pakete senden
                    while (packetQueue.TryDequeue(out IPacket packet))
                    {
                        
                        try
                        {
                            byte[] data = MessagePackSerializer.Serialize(packet);
                            //Console.WriteLine($"Sending packet: {packet.GetType()}, len: {data.Length}");
                            stream.Write(BitConverter.GetBytes(data.Length), 0, 2);
                            stream.Write(data, 0, data.Length);
                            stream.Flush();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error sending packet: {ex.Message}");
                            break;
                        }
                    }

                    // spinning schlecht
                    Thread.Sleep(5);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection error: {ex.Message}");
            }
            finally
            {
                isRunning = false;
                try
                {
                    client.Close();
                }
                catch { }
                
                // Server informieren, dass dieser Spieler die Verbindung verloren hat
                Application.ServerInstance.handlePlayerDisconnect(this);
            }
        }
        public System.Net.EndPoint getRemoteEndPoint()
        {
            return client.Client.RemoteEndPoint;
        }

        public void handlePacket(IPacket packet)
        {
            if(packet is PaddleControlPacket)
            {
                
                PaddleControlPacket paddleControlPacket = (PaddleControlPacket)packet;

                System.Console.WriteLine("PaddleControlPacket received: " + paddleControlPacket.Direction);

                if(player.id == 1)
                {
                    Application.ServerInstance.gameInstance.paddle1.setMotionY((-1.0) * paddleControlPacket.Direction * 60);
                    System.Console.WriteLine($"Paddle1 motionY: {Application.ServerInstance.gameInstance.paddle1.getMotionY()}, posY: {Application.ServerInstance.gameInstance.paddle1.getY()}");
                }else
                {
                    Application.ServerInstance.gameInstance.paddle2.setMotionY((-1.0) * paddleControlPacket.Direction * 60);
                    System.Console.WriteLine($"Paddle2 motionY: {Application.ServerInstance.gameInstance.paddle2.getMotionY()}, posY: {Application.ServerInstance.gameInstance.paddle2.getY()}");
                }

            }else if(packet is PingPacket)
            {
                PingPacket pingPacket = (PingPacket)packet;
                packetQueue.Enqueue(new PingPacket(pingPacket.Id));
            }
        }


    }

    
}
