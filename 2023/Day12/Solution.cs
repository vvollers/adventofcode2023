using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2023.Day12;

[ProblemName("Hot Springs")]
class Solution : Solver
{
    public record Spring(string input, List<int> valid);

    private Dictionary<string, long> cache = new();

    long CachedCalculation(ReadOnlySpan<char> springs, List<int> groups)
    {
        var key = $"{springs},{string.Join(',', groups)}";

        if (!cache.TryGetValue(key, out var value))
        {
            value = CalculateValidPermutations(springs, groups);
            cache[key] = value;
        }

        return value;
    }
    
    long CalculateValidPermutations(ReadOnlySpan<char> remainingCharacters, List<int> remainingGroups)
    {
        if (remainingGroups.Count == 0)
        {
            return remainingCharacters.Contains('#') ? 0 : 1;
        }

        if (remainingCharacters.IsEmpty)
        {
            return 0; 
        }

        switch (remainingCharacters[0])
        {
            case '.': return CachedCalculation(remainingCharacters.Trim('.'), remainingGroups);
            case '?':
                return CachedCalculation(String.Concat(".", remainingCharacters[1..]), remainingGroups) +
                       CachedCalculation(String.Concat("#", remainingCharacters[1..]), remainingGroups);
            case '#':
                var runLength = remainingGroups[0];

                if (remainingCharacters.Length < runLength || remainingCharacters[..runLength].
                        Contains('.'))
                {
                    return 0;
                }

                if (remainingGroups.Count == 1)
                {
                    return CachedCalculation(remainingCharacters[runLength..], remainingGroups[1..]);
                }

                if (remainingCharacters.Length < runLength + 1 || remainingCharacters[runLength] == '#')
                {
                    return 0;
                }

                return CachedCalculation(remainingCharacters[(runLength + 1)..], remainingGroups[1..]);
        }

        return 0;
    }

    public object Solve(string input, int repeat)
    {
        var springs = input.Split("\n").
                            Select(line =>
                                   {
                                       var parts = line.Split(" ");
                                       var valid = parts[1].
                                                   Split(",").
                                                   Select(int.Parse).
                                                   ToList();
                                       valid = Enumerable.Repeat(valid, repeat).
                                                          SelectMany(x => x).
                                                          ToList();
                                       var r = parts[0];
                                       for (var i = 0; i < (repeat - 1); i++)
                                       {
                                           r = r + '?' + parts[0];
                                       }

                                       return new Spring(r, valid);
                                   });

        return springs.Select(o => CachedCalculation(o.input, o.valid)).
                       Sum();
    }

    public object PartOne(string input) => Solve(input, 1);

    public object PartTwo(string input) => Solve(input, 5);
}