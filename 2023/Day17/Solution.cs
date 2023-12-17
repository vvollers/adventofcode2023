using System;
using System.Collections.Generic;
using adventofcode.AdventLib;

namespace AdventOfCode.Y2023.Day17;

[ProblemName("Clumsy Crucible")]
class Solution : Solver {
    public enum Directions
    {
        Up = 1,
        Right = 2,
        Down = 4,
        Left = 8,
    }
    
    public static readonly Dictionary<Directions, Directions[]> OtherDirections = new()
    {
        {Directions.Up, new[] {Directions.Up, Directions.Right, Directions.Left}},
        {Directions.Right, new[] {Directions.Right, Directions.Up, Directions.Down}},
        {Directions.Down, new[] {Directions.Down, Directions.Right, Directions.Left}},
        {Directions.Left, new[] {Directions.Left, Directions.Up, Directions.Down}},
    }; 

    public record State(int X, int Y, Directions Direction, int Steps);
    public bool ReachedGoal(State state, int width, int height, int minSteps) => state.X == width - 1 && state.Y == height - 1 && state.Steps >= minSteps - 1;
    public bool IsOutside(int x, int y, int width, int height) => x < 0 || x == width || y < 0 || y == height;
    
    protected int Solve(byte[][] grid, int minSteps, int maxSteps)
    {
        var queue = new PriorityQueue<State, int>();
        var visited = new HashSet<int>();
        var width = grid[0].Length;
        var height = grid.Length;
        
        queue.Enqueue(new State(0, 0, Directions.Right, 1), 0);
        queue.Enqueue(new State(0, 0, Directions.Down, 1), 0);
        
        while (queue.TryDequeue(out var current, out var accumulatedCost))
        {
            if (ReachedGoal(current, width, height, minSteps))
            {
                return accumulatedCost;
            }

            Directions[] searchDirections = current.Steps < minSteps -1 ? [current.Direction] : OtherDirections[current.Direction];
            
            foreach (var direction in searchDirections)
            {
                var newSteps = 0;
                if (direction == current.Direction)
                {
                    newSteps = current.Steps + 1;
                }

                if (newSteps == maxSteps)
                {
                    continue;
                }
                
                var x = current.X + direction switch
                {
                    Directions.Right => 1,
                    Directions.Left => -1,
                    _ => 0,
                };
                
                var y = current.Y + direction switch
                {
                    Directions.Down => 1,
                    Directions.Up => -1,
                    _ => 0,
                };

                if (IsOutside(x,y, width, height))
                {
                    continue;
                }

                var key = HashCode.Combine(current.X.GetHashCode(), current.Y.GetHashCode(), x.GetHashCode(), y.GetHashCode(), newSteps.GetHashCode());

                if (visited.Add(key))
                {
                    queue.Enqueue(new State(x, y, direction, newSteps), accumulatedCost + grid[y][x]);
                }
            }
        }

        return -1;
    }

    public object PartOne(string input) => Solve(input.ParseToByteGrid(), 0, 3);
    public object PartTwo(string input) => Solve(input.ParseToByteGrid(), 4, 10);
}