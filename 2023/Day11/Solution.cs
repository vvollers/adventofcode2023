using System;
using System.Collections.Generic;
using System.Linq;
using adventofcode.AdventLib;

namespace AdventOfCode.Y2023.Day11;

public record Point(int x, int y);

static class StaticMethods
{
    public static List<Point> ExpandGalaxy(this List<Point> galaxies, int factor)
    {
        var emptyColumns = galaxies.AvailableIndexInList(g => g.x).ToList();
        var emptyRows = galaxies.AvailableIndexInList(g => g.y).ToList();

        return galaxies.Select(g => new Point(
            g.x + emptyColumns.Count(x => x < g.x) * factor,
            g.y + emptyRows.Count(y => y < g.y) * factor)).ToList();
    }
}

[ProblemName("Cosmic Expansion")]
class Solution : Solver
{
    public List<Point> ParseInput(string input) => 
        input.Split("\n")
        .Select(
            (line, y) => line.ToCharArray()
                .Select((c, x) => (c, new Point(x, y)))
                .Where(i => i.c != '.')
                .Select(i => i.Item2)
        )
        .SelectMany(x=>x)
        .ToList();

    public long Distance((Point, Point) pair) =>
        Math.Abs((long)pair.Item2.x - pair.Item1.x) +
        Math.Abs((long)pair.Item2.y - pair.Item1.y);

    public object Solve(string input, int factor) =>
        ParseInput(input)
            .ExpandGalaxy(factor)
            .GenerateCombinations()
            .Sum(Distance);

    public object PartOne(string input) => Solve(input, 1);

    public object PartTwo(string input) => Solve(input, 1_000_000-1);
}