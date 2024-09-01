using System.Text;
using WordRanker;

const int BitsPerLetter = 10;

var words = File.ReadAllLines(args.Length > 0 ? args[0] : "WordLists/words_alpha.txt").Where(w => w.Length >= 3);
List<Anagram> anagrams = ComputeAnagramList(words);

var computer = new Computer();

var cts = new CancellationTokenSource();
var task = computer.ComputeAsync(anagrams);
task.ContinueWith(t => cts.Cancel());
var perf = new PerfCounter(CounterWrite);
await perf.Start(cts.Token);
WriteOutput(task.Result);

string CounterWrite()
{
    var b = new StringBuilder();
    b.Append(computer.Counter);
    b.Append(" / ");
    b.Append(anagrams.Count);
    b.Append(", Graph length = ");
    b.Append(computer.GraphLength);
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

static void WriteOutput(List<WordNode> words)
{
    Dictionary<Anagram, HashSet<Anagram>> writeFormat = [];
    foreach (var word in words)
    {
        var flatten = new HashSet<Anagram>();
        writeFormat.Add(word.Anagram, flatten);
        Recurse(writeFormat, flatten, word);
    }

    static void Recurse(Dictionary<Anagram, HashSet<Anagram>> writeFormat, HashSet<Anagram> flatten, WordNode node)
    {
        foreach (var child in node.Children.Concat(node.IndirectChildren))
        {
            flatten.Add(child.Anagram);

            var flatten1 = new HashSet<Anagram>();
            writeFormat.TryAdd(child.Anagram, flatten1);
            Recurse(writeFormat, flatten1, child);
            flatten.UnionWith(flatten1);
        }
    }

    using StreamWriter writer = new("../../../output.txt");
    foreach ((Anagram word, HashSet<Anagram> children) in writeFormat.OrderBy((kv) => kv.Key))
    {
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