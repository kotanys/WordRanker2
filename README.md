# WordRanker2

Finds all **sub-anagrams** for every word in a dictionary — i.e., for each word
A, lists every word B whose multiset of letters is a subset of A's letters.

## Algorithm

### 1. Letter-multiset fingerprint — 384-bit mask

Each word is encoded into a [384-bit integer](./WordRanker/Int384.cs). 26 lowercase letters fit into the
384 bits at 10 bits per letter — each 10-bit field stores the **count** of that
letter in the word. Use [`maxpprint.py`](./maxpprint.py) to figure out the
required bit field length for each letter.

### 2. Anagram grouping

Words sharing the same key (anagrams) are grouped together into an `Anagram`
record. Each group has a single pre-computed mask.

### 3. Subset test

The subset relation is a single bitwise operation:

```
(a.Mask | b.Mask) == a.Mask
```

> [!IMPORTANT]
> If ORing B's mask into A's mask doesn't change A, then every letter in B
> appears with at most the same count in A — B is a sub-anagram of A.

### 4. Optimisation

Anagram groups are sorted by their mask value. During the nested loop,
iteration over B stops early once `b.Mask >= a.Mask` (since masks are
big-endian, larger masks cannot be subsets).

### 5. Parallelism

`Parallel.ForEach` and thread-local accumulation avoids lock contention.
Completed results are written to a file on disk on the fly, since the
result can grow quite large memory-wise.

## Build and Run

```shell
dotnet build
dotnet run --project WordRanker
```

The program expects a word list (one word per line). See Configuration section.

### Configuration

Edit [`WordRanker/Program.cs`](./WordRanker/Program.cs):

| Setting | What to change |
|---|---|
| Input dictionary | `dotnet run --project WordRanker -- path/to/words.txt`, or change `GlobalConfiguration.InputFile` in [`WordRanker/GlobalConfiguration.cs`](./WordRanker/GlobalConfiguration.cs) |
| Output file | Change `GlobalConfiguration.OutputFile` in [`WordRanker/GlobalConfiguration.cs`](./WordRanker/GlobalConfiguration.cs) |
| Max threads used | Change `GlobalConfiguration.MaxDegreeOfParallelism` in [`WordRanker/GlobalConfiguration.cs`](./WordRanker/GlobalConfiguration.cs) |

### Test

```shell
dotnet test
```
