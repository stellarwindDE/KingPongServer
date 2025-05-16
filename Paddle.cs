namespace KingPongServer
{
    public class Paddle
    {

        private int width;
        private int height;

        private double x;
        private double y;

        private double motionY;

        public Paddle() 
        {
            width = 14;
            height = 60;

            motionY = 0;
        }

        public void setX(double x) { this.x = x; }

        public void setY(double y) { this.y = y; }

        public void setHeight(int height) {  this.height = height; }

        public void setMotionY(double motionY) {  this.motionY = motionY; }

        public double getX() { return x; }
        public double getY() { return y; }
        public double getMotionY() { return motionY; }
        public int getHeight() { return height; }


    }
}