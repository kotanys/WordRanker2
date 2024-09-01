namespace WordRanker;

internal class PerfCounter(Func<string> write)
{
    private readonly Func<string> _write = write;
    private readonly DateTime _startTime = DateTime.Now;
    private readonly (int Left, int Top) cursor = Console.GetCursorPosition();

    public async Task Start(CancellationToken ct)
    {
        bool run = true;
        while (run)
        {
            try
            {
                await Task.Delay(1000, ct);

            }
            catch (OperationCanceledException)
            {
                run = false;
            }
            Console.SetCursorPosition(cursor.Left, cursor.Top);
            Console.Write($"{_write()} @ {(int)(DateTime.Now - _startTime).TotalSeconds} s elapsed");
        }
    }
}
