using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Y2018.Day15;
using AngleSharp.Common;

namespace AdventOfCode.Y2023.Day02;

[ProblemName("Cube Conundrum")]
partial class Solution : Solver {
    private static readonly Regex GameLineRegex = GameLineRegexFunc();
    private static readonly Regex GamePartRegex = GamePartRegexFunc();

    internal struct CubeGame {
        public int Id;
        public List<Dictionary<string, int>> Parts;
        public string Line;
    }

    private CubeGame ParseGame(string game) {
        var gamePart = GameLineRegex.Match(game);
        var id = int.Parse(gamePart.Groups[1].Value);
        var gameParts = gamePart.Groups[2].Value.Split("; ");
        
        var parts = new List<Dictionary<string, int>>();
        foreach(var partSplit in gameParts) {
            var grab = GamePartRegex.Matches(partSplit);
            var part = new Dictionary<string, int>();
            foreach(Match grabMatch in grab) {
                var num = int.Parse(grabMatch.Groups[1].Value);
                var color = grabMatch.Groups[2].Value;
                part[color] = num;
            }
            parts.Add(part);
        }
        return new CubeGame() { Id = id, Parts = parts, Line = game };
    }

    public object PartOne(string input)
    {
        var games = input.Split("\n").
                          Select(ParseGame).
                          ToDictionary(o => o.Id, o => o.Parts);

        Dictionary<string, int> maxColors = new() { { "red", 12 }, { "green", 13 }, { "blue", 14 } };

        return games.Where(
                        game => game.Value.All(
                            gameParts => gameParts.All(part => part.Value <= maxColors[part.Key])
                        )
                    ).Sum(game => game.Key);
    }

    private int MaxOfColor(IEnumerable<Dictionary<string, int>> gameParts, string color) =>
        gameParts.Where(part => part.ContainsKey(color)).
                  Select(part => part[color]).
                  Max();

    private Dictionary<string, int> MinOfColorForGame(IReadOnlyCollection<Dictionary<string, int>> gameParts) =>
        new()
        {
            { "red", MaxOfColor(gameParts, "red") }, 
            { "green", MaxOfColor(gameParts, "green") },
            { "blue", MaxOfColor(gameParts, "blue") },
        };
    
    public object PartTwo(string input) {
        var games = input.Split("\n").
                          Select(ParseGame).
                          ToDictionary(o => o.Line, o => o.Parts);
        
        return games.Select(game => MinOfColorForGame(game.Value)).
                     Select(minColors => minColors["red"] * minColors["green"] * minColors["blue"]).
                     Sum();
    }

    [GeneratedRegex(@"Game (\d+): (.*)")]
    private static partial Regex GameLineRegexFunc();
    [GeneratedRegex(@"(\d+) (\w+)")]
    private static partial Regex GamePartRegexFunc();
}