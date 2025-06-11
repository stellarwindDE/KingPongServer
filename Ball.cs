

using KingPongServer.Network;

namespace KingPongServer
{
    public class Ball
    {
        private const int size = 10;

        private int posX { get; }
        private int posY { get; }

        private double velocityX;
        private double velocityY;
        
        public Ball()
        {
            posY = posX = 0;
            velocityX = velocityY = 0;
        }

        public void setVelocity(double velocityX, double velocityY)
        {
            this.velocityY = velocityY;
            this.velocityX = velocityX;
        }

        public void setVelocityY(double velocityY)
        {
            this.velocityY = velocityY;
        }

        public void setVelocityX(double velocityX)
        {
            this.velocityX = velocityX;
        }
        
    }

}