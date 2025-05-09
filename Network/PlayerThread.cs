using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using MessagePack;

namespace KingPongServer.Network
{
    public class PlayerThread
    {   
        private TcpClient client;
        private Thread thread;
        private ConcurrentQueue<IPacket> packetQueue;
        private bool isRunning;

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
                        try
                        {
                            byte[] buffer = new byte[4096];
                            int bytesRead = stream.Read(buffer, 0, buffer.Length);
                            
                            if (bytesRead == 0)
                            {
                                // Verbindung von Client geschlossen
                                break;
                            }

                            IPacket message = MessagePackSerializer.Deserialize<IPacket>(buffer);
                            // TODO: eingehende pakete verarbeiten
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
    }
}
