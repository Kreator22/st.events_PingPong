using System.Diagnostics;
using System.Reflection.PortableExecutable;

Console.WriteLine("Введите количество раундов");

int balls;
if (!int.TryParse(Console.ReadLine(), out balls) || balls > 10)
    balls = 10;

Ping ping = new(balls);
Pong pong = new();

ping.OnPingPong += pong.ReceiveBall; //pong подписывается на событие ping'а методом ReceiveBall 
pong.OnPingPong += ping.ReceiveBall; //ping подписывается на событие pong'а методом ReceiveBall 

ping.StartPingPong();

//У классов Ping и Pong много одинакового функционала, поэтому для них будет общий
//базовый класс PingPong с реализацией повторяемого функционала
//Класс имеет абстрактный метод BallsLeft без реализации и сам становится абстрактным
abstract class PingPong
{
    //Конструктор базового класса подписывается на событие пустым анонимным методом
    //Теперь событие никогда не выдаст NullReferenceException
    public PingPong() => OnPingPong += (object? p, PingPongEventArgs e) => { };

    //Событие, допускает переопределение в производных классах
    public virtual event EventHandler<PingPongEventArgs> OnPingPong;

    protected virtual void SendBall(PingPongEventArgs e) 
    {
        //Копия события
        EventHandler<PingPongEventArgs> handler = OnPingPong;

        //В многопоточном коде между проверкой на null и вызовом события
        //слушатели могут отписаться от события,
        //но копия хранит все ссылки и не станет равной null
        if (handler != null) handler(this, e);
    }

    public void ReceiveBall(object? p, PingPongEventArgs e) 
    { 
        int ballsLeft = BallsLeft(e);
        
        if (ballsLeft > 0)
        {
            PingPongPrinter(ballsLeft);
            SendBall(new PingPongEventArgs() { Balls = ballsLeft });
        }  
    }

    //Метод должен иметь разную реализацию в производных классах,
    //но реализация для базового класса PingPong не нужна, поэтому метод абстрактный
    protected abstract int BallsLeft(PingPongEventArgs e);
    protected void PingPongPrinter(int ballsLeft) =>
        Console.WriteLine(this.GetType().Name + " " + ballsLeft + " left");
}

class Ping : PingPong
{
    int StartBalls {  get; set; }

    //Цепочка конструкторов с вызовом конструктора базового класса
    public Ping() : this(5) { }
    public Ping(int balls) : base() => StartBalls = balls;

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