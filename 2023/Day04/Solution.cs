using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode.Y2023.Day04;

[ProblemName("Scratchcards")]
partial class Solution : Solver
{
    private static readonly Regex GameLineRegex = GameLineRegexFunc();
    private static readonly Regex GamePartRegex = GamePartRegexFunc();

    public readonly Dictionary<int, int> ExtraScratchCards = new();

    private IEnumerable<int> ParseNumbers(string numbersString) =>
        GamePartRegex.Matches(numbersString).
                      Select(m => int.Parse(m.Value));

    private Tuple<int, int> ParseGameLine(string line)
    {
        var gameParts = GameLineRegex.Match(line);
        var numEqualNumbers = ParseNumbers(gameParts.Groups[3].Value).Intersect(ParseNumbers(gameParts.Groups[4].Value)).Count();
        return new Tuple<int, int>(int.Parse(gameParts.Groups[2].Value), numEqualNumbers);
    }

    private int ParseGame(string line)
    {
        var (cardNumber, numEqualNumbers) = ParseGameLine(line);
        var numCards = 1 + ExtraScratchCards.GetValueOrDefault(cardNumber, 0);

        for (var i = cardNumber + 1; i <= cardNumber + numEqualNumbers; i++)
        {
            ExtraScratchCards[i] = ExtraScratchCards.GetValueOrDefault(i, 0) + numCards;
        }

        return numCards;
    }
    
    public object PartOne(string input)
    {
        return input.Split("\n").
                          Select(line => (int) Math.Pow(2, ParseGameLine(line).Item2-1)).
                          Sum();
    }

    public object PartTwo(string input)
    {
        return input.Split("\n").
                     Select(ParseGame).
                     Sum();
    }
    
    [GeneratedRegex(@"Card(\s+)(\d+): (.*) \| (.*)")]
    private static partial Regex GameLineRegexFunc();
    [GeneratedRegex(@"(\d+)")]
    private static partial Regex GamePartRegexFunc();
}
