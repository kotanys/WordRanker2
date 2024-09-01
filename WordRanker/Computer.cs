namespace WordRanker;

public class Computer
{
    private int _counter = 0;
    private LinkedList<WordNode> _graph = [];

    public int GraphLength => _graph.Count;
    public int Counter => _counter;

    public List<WordNode> Compute(IEnumerable<Anagram> anagrams)
    {
        _graph = [];
        var sorted = anagrams.OrderBy(x => x.Mask).ToArray();
        foreach (Anagram word in sorted)
        {
            var node = new WordNode(word);
            bool addToEnd = true;
            var linkedListNode = _graph.First;
            while (linkedListNode is not null)
            {
                var next = linkedListNode.Next;
                if (node.Anagram.CanMake(linkedListNode.Value.Anagram))
                {
                    node.Children.Add(linkedListNode.Value);
                    _graph.Remove(linkedListNode);
                }
                else
                {
                    Recurse(linkedListNode.Value.Children, node);
                }
                linkedListNode = next;
            }

            if (addToEnd)
            {
                _graph.AddLast(node);
            }
            _counter++;
        }
        return _graph.ToList();
    }

    private static void Recurse(List<WordNode> children, WordNode node)
    {
        var word = node.Anagram;
        foreach (var item in children)
        {
            if (word.CanMake(item.Anagram))
            {
                node.IndirectChildren.Add(item);
            }
            else
            {
                Recurse(item.Children, node);
            }
        }
    }

    public async Task<List<WordNode>> ComputeAsync(IEnumerable<Anagram> anagrams)
    {
        var task = Task.Run(() => Compute(anagrams));
        return await task;
    }
}
