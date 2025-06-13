using KingPongServer.Network;

namespace KingPongServer
{
    public class Game
    {

        public const int SizeY = 600;
        public const int SizeX = 800;

        public Paddle paddle1;
        public Paddle paddle2;
        private Ball ball;

        private Player[] players;

        private long lastUpdateTime;
        private long gameStartTime;

        public GameState state { get; private set; }
        public int countdown { get; private set; }
        private long countdownStart;
        
        public Game() {

            players = new Player[2];

            paddle1 = new Paddle();
            paddle2 = new Paddle();

            ball = new Ball(this);
            ball.spawnInMiddleWithRandomVelocity();

            paddle1.setX(10);
            paddle1.setY(SizeY / 2 - paddle1.getHeight() / 2);

            paddle2.setX(SizeX - 10 - paddle2.getWidth());
            paddle2.setY(SizeY / 2 - paddle2.getHeight() / 2);

        }


        public void StartGame(Player player1, Player player2)
        {
            players[0] = player1;
            players[1] = player2;

            gameStartTime = lastUpdateTime = DateTime.Now.Ticks; //10.000 Ticks = 1ms

            sendChangeGameState(GameState.COUNTDOWN, 5);
            countdownStart = DateTime.Now.Ticks;
            countdown = 5;


            //GameLoop
            while(true){
                if(state == GameState.ENDED){
                    break;
                }
                Update();
                Thread.Sleep(30);
            }
        }


        public void Update(){
            long deltaTime = (DateTime.Now.Ticks - lastUpdateTime) / 10000;

            if(state == GameState.ACTIVE){

                // Paddles
                paddle1.posY += paddle1.motionY * deltaTime/1000;
                paddle2.posY += paddle2.motionY * deltaTime/1000;

                if(paddle1.posY < 0) paddle1.posY = 0;
                if(paddle1.posY + paddle1.getHeight() > SizeY) paddle1.posY = SizeY - paddle1.getHeight();
                if(paddle2.posY < 0) paddle2.posY = 0;
                if(paddle2.posY + paddle2.getHeight() > SizeY) paddle2.posY = SizeY - paddle2.getHeight();



                // Ball
                ball.prepareUpdate((int)deltaTime);

                if(ball.checkCollisionWithPaddle(paddle1) || ball.checkCollisionWithPaddle(paddle2)){
                    ball.setVelocityX(-ball.velocityX);
                }

                if(ball.checkCollisionWithWall()){

                    if(ball.nextPosX + Ball.size > Game.SizeX){
                        scorePlayer(0);
                        return;
                    }
                    else if(ball.nextPosX < 0){
                        scorePlayer(1);
                        return;
                    }


                    ball.setVelocityY(-ball.velocityY);
                }


                ball.updatePosition();

            }else if(state == GameState.COUNTDOWN){
                int current = (int)((0 - countdownStart - (countdown*10000*1000) + DateTime.Now.Ticks) / (10000*1000));
                System.Console.WriteLine("Countdown: " +countdown+ " current: " +current+" start: " +countdownStart + " delta: " + (DateTime.Now.Ticks - countdownStart) + " deltaTime: " + deltaTime);
                if(current >= 0){
                    countdownStart = countdown = 0;
                    changeGameState(GameState.ACTIVE);
                }
                else {
                    sendChangeGameState(GameState.COUNTDOWN, -1*current);
                }
            }else if(state == GameState.ENDED){
                System.Console.WriteLine("Game ended");
                Environment.Exit(0);
            }

            foreach(Player player in players){
                sendCurrentPositions(player);
                //player.playerThread.QueuePacket(new PaddlePositionPacket(paddle1.posX, paddle1.posY, 0, paddle1.motionY, paddle2.posX, paddle2.posY, 0, paddle2.motionY, ball.posX, ball.posY, ball.velocityX, ball.velocityY));
            }

            lastUpdateTime = DateTime.Now.Ticks;
        }

        public void sendCurrentPositions(Player player)
        {
            player.playerThread.QueuePacket(new PaddlePositionPacket(paddle1.posX, paddle1.posY, 0, paddle1.motionY, paddle2.posX, paddle2.posY, 0, paddle2.motionY, ball.posX, ball.posY, ball.velocityX, ball.velocityY));
        }

        public void scorePlayer(int n){
            sendChangeGameState(GameState.COUNTDOWN, 5);
            countdownStart = DateTime.Now.Ticks;
            countdown = 5;

            if(gameStartTime+10000*60000 < DateTime.Now.Ticks){
                changeGameState(GameState.ENDED);
                
            }

            players[n].score++;
            foreach(Player player in players){
                player.playerThread.QueuePacket(new ScorePacket(players[0].score, players[1].score));
            }

            ball.spawnInMiddleWithRandomVelocity();
        }

        public void sendChangeGameState(GameState state, int countdown){
            this.state = state;

            foreach(Player player in players){
                player.playerThread.QueuePacket(new GameStatePacket(state, countdown, player.id));
            }
        }

        public void changeGameState(GameState state){
            sendChangeGameState(state, 0);
        }

    }
}