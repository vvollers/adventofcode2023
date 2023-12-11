using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2023.Day11;

public record Point(int x, int y);

static class StaticMethods
{
    // generates a pair for every unique combination in a list
    public static IEnumerable<(T, T)> GenerateCombinations<T>(this List<T> list) =>
        Enumerable.Range(0, list.Count).Select(i =>
            Enumerable.Range(i + 1, list.Count - i - 1).Select(
                j => (list[i], list[j])
            )
        ).SelectMany(x=>x);

    // returns a list of indexes for a range of 0 -> max(list) where there is no element for that index
    // (based on the getter function which should return a property of an element of the list)
    public static IEnumerable<int> AvailableIndexInList<T>(this List<T> list, Func<T, int> getterFunc) =>
        Enumerable.Range(0, list.Max(getterFunc))
            .Where(x => list.All(g => getterFunc(g) != x));

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