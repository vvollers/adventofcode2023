using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2023.Day03;

[ProblemName("Gear Ratios")]
class Solution : Solver
{
    private static char[][] _grid;
    private static bool[,] _marked;

    public record Coordinate(int X, int Y)
    {
        public char C => _grid[Y][X];
        public bool IsInsideGrid => X >= 0 && Y >= 0 && X < _grid.GetLength(0) && Y < _grid.Length;
        public bool IsNotNumberOrEmpty => IsInsideGrid && !IsNumber && C != '.';
        public bool IsNumber => IsInsideGrid && char.IsDigit(C);
        public bool IsGear => IsInsideGrid && C == '*';
        public bool IsMarked => IsInsideGrid && _marked[Y, X];
        public int NumDigits => (int)Math.Floor(Math.Log10(FindNumber()) + 1);

        public int FindNumber()
        {
            var startx = X;
            var endx = X;
            while ((this with { X = startx }).IsNumber)
            {
                startx--;
            }

            startx++;
            while ((this with { X = endx }).IsNumber)
            {
                endx++;
            }

            endx--;

            for (var x = startx; x <= endx; x++)
            {
                _marked[Y, x] = true;
            }

            return int.Parse(_grid[Y][startx..(endx + 1)]);
        }
    }

    private IEnumerable<Coordinate> GridSelection(int minX, int minY, int maxX, int maxY) =>
        from y in Enumerable.Range(minY, (maxY - minY) + 1)
        from x in Enumerable.Range(minX, (maxX - minX) + 1)
        let c = new Coordinate(x, y)
        where c.IsInsideGrid
        select c;

    private bool NumberIsAdjacentToSymbol(Coordinate coord) =>
        GridSelection(coord.X - 1, coord.Y - 1, coord.X + coord.NumDigits, coord.Y + 1).
            Any(c => c.IsNotNumberOrEmpty);

    public IEnumerable<int> FindAdjacentNumbers(Coordinate coord) =>
        GridSelection(coord.X - 1, coord.Y - 1, coord.X + 1, coord.Y + 1).
            Where(c => c.IsNumber).
            Where(c => !c.IsMarked).
            Select(c => c.FindNumber());
    
    private void InitGrid(string input)
    {
        _grid = input.Split("\n").
                     Select(x => x.ToCharArray()).
                     ToArray();

        _marked = new bool[_grid.Length, _grid[0].Length];
    }

    public object PartOne(string input)
    {
        InitGrid(input);

        return GridSelection(0, 0, _grid[0].Length - 1, _grid.Length - 1).
               Where(c => c.IsNumber).
               Where(c => !c.IsMarked).
               Where(NumberIsAdjacentToSymbol).
               Sum(c => c.FindNumber());
    }

    public object PartTwo(string input)
    {
        InitGrid(input);

        return GridSelection(0, 0, _grid[0].Length - 1, _grid.Length - 1).
               Where(c => c.IsGear).
               Select(n => FindAdjacentNumbers(n).
                          ToList()).
               Where(x => x.Count == 2).
               Sum(x => x.Aggregate(1, (acc, i) => acc * i));
    }
}