using System.Diagnostics;
using System.Reflection.PortableExecutable;

Ping ping = new();
Pong pong = new();

ping.OnPingPong += pong.ReceiveBall;
pong.OnPingPong += ping.ReceiveBall;

ping.StartPingPong();




abstract class PingPong
{
    public virtual event EventHandler<PingPongEventArgs> OnPingPong;

    public virtual void SendBall(PingPongEventArgs e) { OnPingPong(this, e); }
    public void ReceiveBall(object? p, PingPongEventArgs e) 
    { 
        int ballsLeft = BallsLeft(e);
        
        if (ballsLeft > 0)
        {
            PingPongPrinter(ballsLeft);
            SendBall(new PingPongEventArgs() { Balls = ballsLeft });
        }
            
    }
    
    protected abstract int BallsLeft(PingPongEventArgs e);
    protected void PingPongPrinter(int ballsLeft) =>
        Console.WriteLine(this.GetType().Name + " " + ballsLeft + " left");

}

class Ping : PingPong
{
    int StartBalls {  get; set; }
    public Ping() : this(5) { }
    public Ping(int balls) => StartBalls = balls;

    public void StartPingPong()
    {
        PingPongPrinter(StartBalls);
        SendBall(new PingPongEventArgs() { Balls = StartBalls });
    }

    protected override int BallsLeft(PingPongEventArgs e) => e.Balls - 1;

}

class Pong : PingPong
{
    protected override int BallsLeft(PingPongEventArgs e) => e.Balls;
}

class PingPongEventArgs : EventArgs
{
    public int Balls { get; init; }
}