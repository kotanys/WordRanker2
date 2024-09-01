using System.Collections.Concurrent;

namespace WordRanker;

internal class Computer
{
    private int _counter;

    public int Counter => _counter;
    public ConcurrentQueue<List<WordNode>> Results { get; private set; } = [];

    public async Task Compute(IEnumerable<Anagram> anagrams, ParallelOptions options)
    {
        var possiblePlaces = new ConcurrentStack<int>(Enumerable.Range(0, options.MaxDegreeOfParallelism));
        var list = anagrams.OrderBy(a => a.Mask).ToList();

        await Task.Run(() => Parallel.ForEach(anagrams, options,
        GetLocalInit(),
        GetBody(list),
        GetLocalFinally()));
    }

    private Action<ThreadWorkState> GetLocalFinally()
    {
        return s =>
        {
            Interlocked.Add(ref _counter, s.LocalCounter);
            Results.Enqueue(s.Nodes ?? throw new InvalidOperationException());
        };
    }

    private Func<Anagram, ParallelLoopState, ThreadWorkState, ThreadWorkState> GetBody(List<Anagram> list)
    {
        return (a, state, s) =>
        {
            var node = new WordNode(a);
            foreach (var b in list)
            {
                if (b.Mask >= a.Mask)
                {
                    break;
                }
                if ((a.Mask | b.Mask) == a.Mask)
                {
                    node.Children.Add(new WordNode(b));
                }
            }
            s.LocalCounter++;
            s.Nodes.Add(node);
            return s;
        };
    }

    private static Func<ThreadWorkState> GetLocalInit()
    {
        return () =>
        {
            return (Nodes: [], LocalCounter: 0);
        };
    }
}

internal record struct ThreadWorkState(List<WordNode> Nodes, int LocalCounter)
{
    public static implicit operator (List<WordNode> Nodes, int LocalCounter)(ThreadWorkState value)
    {
        return (value.Nodes, value.LocalCounter);
    }

    public static implicit operator ThreadWorkState((List<WordNode> Nodes, int LocalCounter) value)
    {
        return new ThreadWorkState(value.Nodes, value.LocalCounter);
    }
}