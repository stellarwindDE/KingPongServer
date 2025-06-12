

using KingPongServer.Network;

namespace KingPongServer
{
    public class Ball
    {
        public const int size = 5;

        public double posX { get; private set; }
        public double posY { get; private set; }

        public double nextPosX { get; private set; }
        public double nextPosY { get; private set; }

        public double velocityX { get; private set; }
        public double velocityY { get; private set; }

        private double speed = 40; //px/s

        private Game game;
        
        public Ball(Game game)
        {
            this.game = game;
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


        public void spawnInMiddleWithRandomVelocity(){
            posX = Game.SizeX / 2 - size / 2;
            posY = Game.SizeY / 2 - size / 2;

            double angle = Random.Shared.NextDouble() * 2 * Math.PI; // 2rad = 360°
            setVelocityByAngle(angle);

        }

        //Winkel in radians
        public void setVelocityByAngle(double angle){
            velocityX = Math.Cos(angle) * speed;
            velocityY = Math.Sin(angle) * speed;
        }

        public void prepareUpdate(int deltaTime){
            nextPosX = posX + velocityX * deltaTime/1000;
            //System.Console.WriteLine($"posX: [{posX}] nextPosX: [{nextPosX}] velocityX: [{velocityX}] deltaTime: [{deltaTime}]");
            nextPosY = posY + velocityY * deltaTime/1000;
        }

        public bool checkCollisionWithPaddle(Paddle paddle){
            if(nextPosY + size > paddle.posY && nextPosY < paddle.posY + paddle.getHeight()){
                if(nextPosX + size > paddle.posX && nextPosX < paddle.posX + paddle.getWidth()){
                    return true;
                }
            }
            return false;
        }

        public bool checkCollisionWithWall(){
            if(nextPosX + size > Game.SizeX || nextPosX < 0){
                return true;
            }
            if(nextPosY + size > Game.SizeY || nextPosY < 0){
                return true;
            }
            return false;
        }

        public void updatePosition(){
            posX = nextPosX;
            posY = nextPosY;
        }
    }

}