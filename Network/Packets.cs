using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingPongServer.Network
{

    [Union(0, typeof(GameStatePacket))]
    [Union(1, typeof(ScorePacket))]
    [Union(2, typeof(MessagePacket))]
    [Union(3, typeof(BallPositionPacket))]
    [Union(4, typeof(PaddlePositionPacket))]
    [Union(5, typeof(PaddleControlPacket))]
    [Union(6, typeof(PingPacket))]
    [Union(7, typeof(AuthPacket))]
    public interface IPacket
    {
    }

    //Server Messages

    [MessagePackObject]
    public class GameStatePacket : IPacket
    {
        public GameStatePacket(int State, int Countdown)
        {
            this.State = State;
            this.Countdown = Countdown;
        }
        public GameStatePacket(GameState State, int Countdown)
        {
            this.State = (int)State;
            this.Countdown = Countdown;
        }
        public GameStatePacket(GameState State)
        {
            this.State = (int)State;
            this.Countdown = 0;
        }

        [Key(0)]
        public int State { get; }
        [Key(1)]
        public int Countdown { get; }
    }

    [MessagePackObject]
    public class ScorePacket : IPacket
    {
        public ScorePacket(int PlayerOneScore, int PlayerTwoScore)
        {
            this.PlayerOneScore = PlayerOneScore;
            this.PlayerTwoScore = PlayerTwoScore;
        }

        [Key(0)]
        public int PlayerOneScore { get; }
        [Key(1)]
        public int PlayerTwoScore { get; }
    }


    [MessagePackObject]
    public class MessagePacket : IPacket
    {
        public MessagePacket(string Text)
        {
            this.Text = Text;
        }

        [Key(0)]
        public string Text { get; }
    }


    [MessagePackObject]
    public class BallPositionPacket : IPacket
    {
        public BallPositionPacket(double X, double Y, double VelocityX, double VelocityY)
        {
            this.X = X;
            this.Y = Y;
            this.VelocityX = VelocityX;
            this.VelocityY = VelocityY;
        }

        [Key(0)]
        public double X { get; }
        [Key(1)]
        public double Y { get; }

        [Key(2)]
        public double VelocityX { get; }
        [Key(3)]
        public double VelocityY { get; }
    }


    [MessagePackObject]
    public class PaddlePositionPacket : IPacket
    {
        public PaddlePositionPacket(double PaddleOneX, double PaddleOneY, double PaddleOneVelocityX, double PaddleOneVelocityY, double PaddleTwoX, double PaddleTwoY, double PaddleTwoVelocityX, double PaddleTwoVelocityY)
        {
            this.PaddleOneX = PaddleOneX;
            this.PaddleOneY = PaddleOneY;
            this.PaddleOneVelocityX = PaddleOneVelocityX;
            this.PaddleOneVelocityY = PaddleOneVelocityY;

            this.PaddleTwoX = PaddleTwoX;
            this.PaddleTwoY = PaddleTwoY;
            this.PaddleTwoVelocityX = PaddleTwoVelocityX;
            this.PaddleTwoVelocityY = PaddleTwoVelocityY;
        }


        [Key(0)]
        public double PaddleOneX { get; }
        [Key(1)]
        public double PaddleOneY { get; }

        [Key(2)]
        public double PaddleOneVelocityX { get; }
        [Key(3)]
        public double PaddleOneVelocityY { get; }


        [Key(4)]
        public double PaddleTwoX { get; }
        [Key(5)]
        public double PaddleTwoY { get; }

        [Key(6)]
        public double PaddleTwoVelocityX { get; }
        [Key(7)]
        public double PaddleTwoVelocityY { get; }
    }




    //Client Messages

    [MessagePackObject]
    public class PaddleControlPacket : IPacket
    {
        public PaddleControlPacket(short Direction)
        {
            this.Direction = Direction;
        }

        [Key(0)]
        public short Direction { get; }
    }


    [MessagePackObject]
    public class PingPacket : IPacket
    {
        public PingPacket(int Id)
        {
            this.Id = Id;
        }

        [Key(0)]
        public int Id { get; }
    }


    [MessagePackObject]
    public class AuthPacket : IPacket
    {
        public AuthPacket(string Text)
        {
            this.Text = Text;
        }

        [Key(0)]
        public string Text { get; }
    }
}
