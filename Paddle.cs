namespace KingPongServer
{
    public class Paddle
    {

        private int width;
        private int height;

        public double posX;
        public double posY;

        public double motionY { get; private set; }

        public Paddle() 
        {
            width = 10;
            height = 120;

            motionY = 0;
        }

        public void setX(int x) { this.posX = x; }

        public void setY(int y) { this.posY = y; }

        public void setHeight(int height) {  this.height = height; }

        public void setMotionY(double motionY) {  this.motionY = motionY; }

        public double getX() { return posX; }
        public double getY() { return posY; }
        public double getMotionY() { return motionY; }
        public int getHeight() { return height; }
        public int getWidth() { return width; }


    }
}