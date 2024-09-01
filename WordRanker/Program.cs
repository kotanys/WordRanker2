using System.Text;
using WordRanker;

const int BitsPerLetter = 10;

var words = File.ReadAllLines(args.Length > 0 ? args[0] : GlobalConfiguration.InputFile).Where(w => w.Length >= 3);
List<Anagram> anagrams = ComputeAnagramList(words);


var computer = new Computer();
var perf = new PerfCounter(CounterWrite);
perf.Start();

var task = computer.StartCompute(anagrams, new ParallelOptions { MaxDegreeOfParallelism = 20 });
using var writer = new StreamWriter(GlobalConfiguration.OutputFile);

while (!task.IsCompleted || !computer.Results.IsEmpty)
{
    if (computer.Results.TryDequeue(out var results))
        WriteOutput(results, writer);
}

perf.WriteNow();
perf.Stop();
await writer.FlushAsync();

string CounterWrite()
{
    var b = new StringBuilder();
    b.Append(computer.Counter);
    b.Append(" / ");
    b.Append(anagrams.Count);
    return b.ToString();
}

static HashSet<char> GetAlphabet(IEnumerable<string> words)
{
    var set = new HashSet<char>();
    foreach (var word in words)
    {
        foreach (var letter in word)
        {
            if (!char.IsLetter(letter))
                continue;
            set.Add(char.ToLower(letter));
        }
    }
    return set;
}

static List<Anagram> GetAnagrams(IEnumerable<string> words, int minCode)
{
    var d = new Dictionary<string, Anagram>();
    foreach (var word in words)
    {
        var sorted = new string(word.Order().ToArray());
        if (d.ContainsKey(sorted))
        {
            d[sorted].Words.Add(word);
        }
        else
        {
            var mask = GetMask(word, minCode);
            if (!mask.HasValue)
            {
                Console.WriteLine($"Discarding word {word}");
                continue;
            }
            var anagram = new Anagram([word], mask.Value);
            d.Add(sorted, anagram);
        }
    }
    return d.Values.ToList();
}

static Int384? GetMask(string word, int minCode)
{
    Dictionary<char, int> counts = [];
    foreach (var letter in word)
    {
        if (!char.IsLetter(letter))
            continue;

        var lowerletter = char.ToLower(letter);
        if (!counts.ContainsKey(lowerletter))
            counts.Add(lowerletter, 1);
        else
            counts[lowerletter]++;
    }
    if (counts.Values.Any(x => x > BitsPerLetter))
    {
        return null;
    }

    var m = new Int384();
    foreach ((char letter, int count) in counts)
    {
        m |= new Int384(0, 0, 0, 0, 0, (ulong)Math.Pow(2, count) - 1) << ((letter - minCode) * BitsPerLetter);
    }
    return m;
}

static List<Anagram> ComputeAnagramList(IEnumerable<string> words)
{
    var alphabet = GetAlphabet(words);
    var minCode = (int)alphabet.Min();
    var anagrams = GetAnagrams(words, minCode);
    anagrams.Sort((x, y) => x.Mask.CompareTo(y.Mask));
    return anagrams;
}

static void WriteOutput(List<WordNode> words, StreamWriter writer)
{
    foreach (var node in words)
    {
        var word = node.Anagram;
        var children = node.Children;
        writer.Write("{");
        writer.Write(word);
        writer.Write("}: ");
        foreach (var child in children)
        {
            writer.Write(child);
            writer.Write(", ");
        }
        writer.WriteLine();
    }
}