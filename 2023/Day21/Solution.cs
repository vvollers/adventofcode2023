using System;
using System.Collections.Generic;
using System.Linq;
using adventofcode.AdventLib;

namespace AdventOfCode.Y2023.Day21;

public enum Directions
{
    Up = 1,
    Right = 2,
    Down = 4,
    Left = 8,
}

public record Location(int X, int Y)
{
    public Location Move(Directions dir, int dist = 1)
    {
        return dir switch
               {
                   Directions.Up => new Location(this.X - dist, this.Y),
                   Directions.Down  => new Location(this.X + dist, this.Y),
                   Directions.Right  => new Location(this.X, this.Y + dist),
                   Directions.Left  => new Location(this.X, this.Y - dist),
               };
    }
}
[ProblemName("Step Counter")]
class Solution : Solver
{
    public static Directions[] AllDirections = [Directions.Up, Directions.Down, Directions.Right, Directions.Left];
    
    public Location FindLocation(char[][] grid, char c)
    {
        for (var yy = 0; yy < grid.Length; yy++)
        {
            for(var xx =0; xx < grid[yy].Length; xx++)
            {
                if (grid[yy][xx] == c)
                {
                    return new Location(xx, yy);
                }
            }
        }
        
        return new Location(-1, -1);
    }
    
    public bool IsInsideGrid(Location loc, char[][] grid) => loc.X >= 0 && loc.Y >= 0 && loc.Y < grid.Length && loc.X < grid[0].Length;

    IEnumerable<Location> WalkEveryDirection(HashSet<Location> currentPositions, Func<Location, bool> predicate)
    {
        return currentPositions.SelectMany(it => AllDirections.Select(dir => it.Move(dir))).
                                      Where(predicate);
    }       

    public HashSet<Location> WalkEveryDirectionNumberOfTimes(int numberOfTimes, char[][] grid, HashSet<Location> currentPositions, Func<Location, bool> predicate)
    {
        var positions = currentPositions;
        for (var i = 0; i < numberOfTimes; i++)
        {
            positions = [..WalkEveryDirection(positions, predicate)];
        }

        return positions;
    }
    
    public int GetNumberOfWalkOptions(char[][] grid, Location loc, int maxSteps)
    {
        var positions = new HashSet<Location> { loc };
        
        positions = WalkEveryDirectionNumberOfTimes(maxSteps, grid, positions, location => IsInsideGrid(location, grid) && grid[location.Y][location.X] != '#');

        return positions.Count;
    }
    
    public object PartOne(string input)
    {
        var grid = input.ParseToCharGrid();
        var start = FindLocation(grid, 'S');
        
        return GetNumberOfWalkOptions(grid, start, 64);
    }
    
    public long Quadratic(long n, long a, long b, long c) => a * (n*n) + b * n + c;
    
    public int PositiveModulo(int x, int m) => (x % m + m) % m;
    
    public char GetCharInInfiniteGrid(Location loc, char[][] grid) => grid[PositiveModulo(loc.Y, grid.Length)][PositiveModulo(loc.X, grid[0].Length)];
    
    public object PartTwo(string input) {
        var grid = input.ParseToCharGrid();
        var start = FindLocation(grid, 'S');

        var numberOfRequestedSteps = 26501365;
        
        var gridSize = grid.Length;
        
        var numberOfGrids = numberOfRequestedSteps / gridSize;
        var remainder = numberOfRequestedSteps % gridSize;

        var locations = new HashSet<Location> { start };
        
        locations = WalkEveryDirectionNumberOfTimes(remainder, grid, locations, loc => GetCharInInfiniteGrid(loc, grid) != '#');
        var c = locations.Count;

        locations = WalkEveryDirectionNumberOfTimes(gridSize, grid, locations, loc => GetCharInInfiniteGrid(loc, grid) != '#');
        var aPlusBPlusC = locations.Count;

        locations = WalkEveryDirectionNumberOfTimes(gridSize, grid, locations, loc => GetCharInInfiniteGrid(loc, grid) != '#');
        var fourAPlusTwoBPlusC = locations.Count;
        
        /***
         *
         * 
         * The selected code is part of a method that calculates a quadratic sequence.
         * The sequence is based on the number of steps taken in a grid, and the code is trying to find a pattern in
         * this sequence to predict future values without having to calculate each step individually.
         *
         * The first line var c 
         * This value represents the constant term in the quadratic equation.
         *
         * The next line var aPlusB = aPlusBPlusC - c;
         * This difference represents the sum of the coefficients of the linear and  quadratic terms in the quadratic
         * equation.
         *
         * The third line var fourAPlusTwoB = fourAPlusTwoBPlusC - c;
         * This difference represents four times the coefficient of the quadratic term  plus two times the coefficient of t
         * he linear term.
         *
         * The fourth line var twoA = fourAPlusTwoB - (2 * aPlusB); is subtracting twice the sum of the coefficients
         * of the linear and quadratic terms from the previously calculated difference. This gives us twice the
         * coefficient of the quadratic term.  The fifth line var a = twoA / 2; is simply dividing the previous
         * result by two to get the coefficient of the quadratic term.
         *
         * The last line var b = aPlusB - a; is subtracting the coefficient of the quadratic term from the sum of the
         * coefficients of the linear and quadratic terms to get the coefficient of the linear term.
         *
         * In summary, this code is using the first three values in the sequence to calculate the coefficients of
         * a quadratic equation that can be used to predict future values in the sequence.
         */

        var aPlusB = aPlusBPlusC - c;               // a + b + c
        var fourAPlusTwoB = fourAPlusTwoBPlusC - c; // 4a +2b +c
        var twoA = fourAPlusTwoB - (2 * aPlusB);
        var a = twoA / 2;
        var b = aPlusB - a;

        return Quadratic(numberOfGrids,a,b,c);
    }
}
