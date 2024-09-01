using System.Collections.Concurrent;

namespace WordRanker;

internal class Computer
{
    private int _counter;

    public int Counter => _counter;
    public ConcurrentQueue<List<WordNode>> Results { get; private set; } = [];

    public async Task StartCompute(IEnumerable<Anagram> anagrams, ParallelOptions options)
    {
        var list = anagrams.OrderBy(a => a.Mask).ToList();

        await Task.Run(() => Parallel.ForEach(anagrams,
                                              options,
                                              LocalInit,
                                              GetBody(list),
                                              LocalFinally));
    }

    private static ThreadWorkState LocalInit() => new(Nodes: [], LocalCounter: 0);

    private static Func<Anagram, ParallelLoopState, ThreadWorkState, ThreadWorkState> GetBody(List<Anagram> list)
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

    private void LocalFinally(ThreadWorkState s)
    {
        Interlocked.Add(ref _counter, s.LocalCounter);
        Results.Enqueue(s.Nodes ?? throw new InvalidOperationException());
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