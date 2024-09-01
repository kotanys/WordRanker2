namespace WordRanker;

internal class PerfCounter(Func<string> write)
{
    private readonly Func<string> _write = write;
    private readonly (int Left, int Top) cursor = Console.GetCursorPosition();
    private DateTime _startTime = DateTime.Now;
    private Timer? _timer;
    public void Start()
    {
        if (_timer is not null)
            Stop();

        _startTime = DateTime.Now;
        _timer = new Timer(Callback, null, 10, 1000);

        void Callback(object? state) => WriteNow();
    }

    public void WriteNow()
    {
        Console.SetCursorPosition(cursor.Left, cursor.Top);
        Console.Write($"{_write()} @ {(int)(DateTime.Now - _startTime).TotalSeconds} s elapsed");
    }

    public void Stop()
    {
        _timer?.Dispose();
    }
}
