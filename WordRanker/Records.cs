namespace WordRanker;

public record WordNode(Anagram Anagram)
{
    public List<WordNode> Children { get; init; } = [];

    public override string ToString()
    {
        return Anagram.ToString();
    }
}

public record Anagram(List<string> Words, Int384 Mask) : IComparable<Anagram>
{
    public int Length => Words[0].Length;

    public override string ToString()
    {
        return string.Join(", ", Words);
    }

    public override int GetHashCode()
    {
        return Mask.GetHashCode();
    }

    public int CompareTo(Anagram? other)
    {
        return Length.CompareTo(other?.Length ?? throw new NullReferenceException());
    }
}