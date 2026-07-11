namespace WordRanker;

public static class GlobalConfiguration
{
    public static string InputFile { get; set; } = "WordLists/words_alpha.txt";
    public static string OutputFile { get; set; } = "../../../output.txt";
    public static int MaxDegreeOfParallelism { get; set; } = 16;
}
