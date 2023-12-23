using System;
using System.Collections.Generic;
using System.Linq;
using adventofcode.AdventLib;

namespace AdventOfCode.Y2023.Day23;

public enum Directions
{
    Up = 1,
    Right = 2,
    Down = 4,
    Left = 8,
    None = 16
}

public record Junction(int X, int Y, Dictionary<Junction, int> Neighbours)
{
    public override string ToString()
    {
        return $"Junction: (row: {Y + 1}, col: {X + 1}) [{X},{Y}] Neighbours: {Neighbours.Count}: {string.Join(", ", Neighbours.Select(kv => $"{kv.Key.X},{kv.Key.Y} = {kv.Value}"))}";
    }
}

public record Location
{
    public Location(int X, int Y, Directions Dir, Location Previous = null)
    {
        this.X = X;
        this.Y = Y;
        this.Dir = Dir;
        this.Previous = Previous;
        this.IsEatingItself = false;
        if (Previous != null)
        {
            IsEatingItself = Previous.History.Contains((X, Y));
            History = Previous.History.ToHashSet();
            History.Add((X, Y));
        }
        else
        {
            History = new HashSet<(int, int)>();
        }
    }

    public Location Move(Directions dir, int dist = 1)
    {
        return dir switch
               {
                   Directions.Left  => new Location(X - dist, Y, Directions.Left, this),
                   Directions.Right => new Location(X + dist, Y, Directions.Right, this),
                   Directions.Down  => new Location(X, Y + dist, Directions.Down, this),
                   Directions.Up    => new Location(X, Y - dist, Directions.Up, this),
                   Directions.None  => new Location(X, Y, Directions.None, this),
                   _                => throw new ArgumentOutOfRangeException(nameof(dir), dir, null)
               };
    }

    public int X { get; init; }
    public int Y { get; init; }
    public Directions Dir { get; init; }
    public Location Previous { get; init; }

    public HashSet<(int, int)> History { get; init; }

    public bool IsEatingItself { get; init; }

    public string ShortString => $"({Y + 1}, {X + 1}) [{X},{Y}] {Dir}";

    public override string ToString()
    {
        return $"(row: {Y + 1}, col: {X + 1}) [{X},{Y}] {Dir} .. Previous: {(Previous != null ? Previous.ShortString : "")} ";
    }
}

[ProblemName("A Long Walk")]
class Solution : Solver
{
    public const char Slopedown = 'v';
    public const char SlopeRight = '>';
    public static readonly char[] Slopes = [Slopedown, SlopeRight];

    public static readonly Dictionary<Directions, Directions[]> OtherDirections = new()
                                                                                  {
                                                                                      { Directions.Up, [Directions.Up, Directions.Right, Directions.Left] },
                                                                                      { Directions.Right, [Directions.Right, Directions.Up, Directions.Down] },
                                                                                      { Directions.Down, [Directions.Down, Directions.Right, Directions.Left] },
                                                                                      { Directions.Left, [Directions.Left, Directions.Up, Directions.Down] },
                                                                                  };

    public bool IsInsideGrid(Location loc, char[][] grid) => loc.X >= 0 && loc.Y >= 0 && loc.Y < grid.Length && loc.X < grid[0].Length;

    IEnumerable<Location> WalkOnInAnyDirection(HashSet<Location> currentPositions, Func<Location, bool> predicate)
    {
        return currentPositions.SelectMany(it => OtherDirections[it.Dir].
                                               Select(dir => it.Move(dir))).
                                Where(predicate);
    }

    public void WalkEveryDirectionUntil(HashSet<Location> currentPositions, Func<Location, bool> predicate)
    {
        var positions = currentPositions;
        while (positions.Count > 0)
        {
            positions = [..WalkOnInAnyDirection(positions, predicate)];
        }
    }

    public bool GridEvaluator(char[][] grid, Location loc, bool considerSlopes)
    {
        if (!IsInsideGrid(loc, grid))
        {
            return false;
        }

        char c = grid[loc.Y][loc.X];
        if (c == '#')
        {
            return false;
        }

        if (loc.IsEatingItself)
        {
            return false;
        }

        if (considerSlopes)
        {
            if (c == Slopedown && loc.Dir != Directions.Down)
            {
                return false;
            }

            if (c == SlopeRight && loc.Dir != Directions.Right)
            {
                return false;
            }
        }

        return true;
    }


    public List<Junction> GetJunctions(char[][] grid, bool considerSlopes)
    {
        List<Junction> junctions = new();
        for (var yy = 1; yy < grid.Length - 1; yy++)
        {
            for (var xx = 1; xx < grid[yy].Length - 1; xx++)
            {
                var c = grid[yy][xx];
                if (c == '.')
                {
                    var numSlopes = 0;
                    for (var y1 = -1; y1 <= 1; y1++)
                    {
                        for (var x1 = -1; x1 <= 1; x1++)
                        {
                            if (x1 == 0 && y1 == 0)
                            {
                                continue;
                            }

                            if (Slopes.Contains(grid[yy + y1][xx + x1]))
                            {
                                numSlopes++;
                            }
                        }
                    }

                    if (numSlopes > 1)
                    {
                        junctions.Add(new Junction(xx, yy, new()));
                    }
                }
            }
        }

        var entryJunction = new Junction(1, 0, new Dictionary<Junction, int>());
        var exitJunction = new Junction(grid[0].Length - 2, grid.Length - 1, new Dictionary<Junction, int>());
        
        junctions.Add(entryJunction);
        junctions.Add(exitJunction);

        foreach (var junction in junctions)
        {
            HashSet<Location> initial = new HashSet<Location>();
            initial.Add(new Location(junction.X, junction.Y, Directions.Up));
            initial.Add(new Location(junction.X, junction.Y, Directions.Down));
            initial.Add(new Location(junction.X, junction.Y, Directions.Left));
            initial.Add(new Location(junction.X, junction.Y, Directions.Right));

            Dictionary<Junction, int> neighbours = new();
            WalkEveryDirectionUntil(initial, loc =>
                                             {
                                                 if (!GridEvaluator(grid, loc, considerSlopes))
                                                 {
                                                     return false;
                                                 }

                                                 if (junctions.TryGetValue(j => j.X == loc.X && j.Y == loc.Y, out var otherJunction))
                                                 {
                                                     neighbours[otherJunction] = loc.History.Count;
                                                     return false;
                                                 }

                                                 return true;
                                             });
            foreach (var kv in neighbours)
            {
                junction.Neighbours[kv.Key] = kv.Value;
            }
        }

        if(!considerSlopes) {
            foreach (var j in junctions)
            {
                var validNeighbours = j.Neighbours.Where(kv => kv.Key != entryJunction && kv.Key != exitJunction).
                                        ToDictionary();
                if (validNeighbours.Count > 3)
                {
                    var shortest = validNeighbours.OrderBy(kv => kv.Value).Select(o => o.Key).First();
                    j.Neighbours.Remove(shortest);
                }
            }
        }

        return junctions;
    }

    public List<List<Junction>> FindPaths(Junction current, Junction to, List<Junction> junctions, List<Junction> history)
    {
        List<List<Junction>> paths = new();
        foreach (var j in current.Neighbours.Keys.Where(kv => !history.Contains(kv)))
        {
            List<Junction> newHistory = [..history, j];
            if (j == to)
            {
                paths.Add(newHistory);
                break;
            }

            var newPaths = FindPaths(j, to, junctions, newHistory);
            paths.AddRange(newPaths);
        }

        return paths;
    }

    public int FindPathLength(List<Junction> path)
    {
        int length = 0;
        for (var i = 0; i < path.Count - 1; i++)
        {
            length += path[i].
                Neighbours[path[i + 1]];
        }

        return length;
    }

    public int Solve(string input, bool considerSlopes)
    {
        var grid = input.ParseToCharGrid();
        var junctions = GetJunctions(grid, considerSlopes);

        var startJunction = junctions.First(j => j.X == 1 && j.Y == 0);
        var endJunction = junctions.First(j => j.X == grid[0].Length - 2 && j.Y == grid.Length - 1);

        Console.WriteLine("Finding paths: ");
        var paths = FindPaths(startJunction, endJunction, junctions, [startJunction]);
        Console.WriteLine("\nFound paths: " + paths.Count);
        return FindPathLength(paths.MaxBy(FindPathLength));
    }

    public object PartOne(string input) => Solve(input, true);

    public object PartTwo(string input) => Solve(input, false);
}