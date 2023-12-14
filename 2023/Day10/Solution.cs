using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2023.Day10;

[ProblemName("Pipe Maze")]
internal class Solution : Solver
{
    public enum PipeType
    {
        None = '.',
        Vertical = '|',
        Horizontal = '-',
        NorthEast = 'L',
        NorthWest = 'J',
        SouthWest = '7',
        SouthEast = 'F',
        Start = 'S',
    }

    public enum RelativeDirection
    {
        SouthEast,
        South,
        SouthWest,
        East,
        Center,
        West,
        NorthEast,
        North,
        NorthWest,
    }

    private static char[][] _grid;
    private static bool[][] _outsideLoop;
    private static bool[][] _insideLoop;
    private static bool[][] _isLoop;

    private static readonly Dictionary<PipeType, char> PipeChars = new()
                                                                   {
                                                                       { PipeType.None, '·' },
                                                                       { PipeType.Horizontal, '\u2550' },
                                                                       { PipeType.Vertical, '\u2551' },
                                                                       { PipeType.NorthEast, '\u255a' },
                                                                       { PipeType.NorthWest, '\u255d' },
                                                                       { PipeType.SouthWest, '\u2557' },
                                                                       { PipeType.SouthEast, '\u2554' },
                                                                       { PipeType.Start, 'S' },
                                                                   };

    private static readonly Dictionary<PipeType, List<RelativeDirection>> ValidDirections = new()
                                                                                            {
                                                                                                { PipeType.Horizontal, [RelativeDirection.East, RelativeDirection.West] },
                                                                                                { PipeType.Vertical, [RelativeDirection.North, RelativeDirection.South] },
                                                                                                { PipeType.NorthEast, [RelativeDirection.North, RelativeDirection.East] },
                                                                                                { PipeType.NorthWest, [RelativeDirection.North, RelativeDirection.West] },
                                                                                                { PipeType.SouthWest, [RelativeDirection.South, RelativeDirection.West] },
                                                                                                { PipeType.SouthEast, [RelativeDirection.South, RelativeDirection.East] },
                                                                                                { PipeType.Start, [RelativeDirection.East, RelativeDirection.West, RelativeDirection.North, RelativeDirection.South] },
                                                                                            };

    public object PartOne(string input)
    {
        InitGrid(input);

        return Math.Floor(Loop().
                              Count() / 2.0);
    }

    public object PartTwo(string input)
    {
        InitGrid(input);
        PrintGrid();

        foreach (var c in Loop())
        {
            _isLoop[c.Y][c.X] = true;
        }

        _outsideLoop = DumbScaleGrid();

        Fill(0, 0);

        ApplyGrid(_insideLoop, (x, y) => !_isLoop[y][x] && !_outsideLoop[y * 3 + 1][x * 3 + 1]);

        PrintFinalGrid();

        return _insideLoop.Sum(row => row.Count(c => c));
    }

    private IEnumerable<Coordinate> GridSelection(int minX, int minY, int maxX, int maxY)
    {
        return from y in Enumerable.Range(minY, maxY - minY + 1)
               from x in Enumerable.Range(minX, maxX - minX + 1)
               let c = new Coordinate(x, y)
               where c.IsInsideGrid
               select c;
    }

    public Coordinate FindStart()
    {
        return GridSelection(0, 0, _grid[0].Length - 1, _grid.Length - 1).
            First(c => c.IsStart);
    }

    public void InitGrid(string input)
    {
        _grid = input.Split("\n").
                      Select(x => x.ToCharArray()).
                      ToArray();
        _outsideLoop = new bool[_grid.Length][];
        _insideLoop = new bool[_grid.Length][];
        _isLoop = new bool[_grid.Length][];
        for (var y = 0; y < _grid.Length; y++)
        {
            _outsideLoop[y] = new bool[_grid[y].Length];
            _insideLoop[y] = new bool[_grid[y].Length];
            _isLoop[y] = new bool[_grid[y].Length];
        }
    }

    public bool Inside(int X, int Y)
    {
        return X >= 0 && Y >= 0 && X < _outsideLoop[0].Length && Y < _outsideLoop.Length && !_outsideLoop[Y][X];
    }

    public void Fill(int xx, int yy)
    {
        // floodfill based on algorithm from wikipedia
        if (_outsideLoop[yy][xx] || _isLoop[yy][xx])
        {
            return;
        }

        Queue<FillCoordinate> queue = new();
        queue.Enqueue(new FillCoordinate(xx, xx, yy, 1));
        queue.Enqueue(new FillCoordinate(xx, xx, yy - 1, -1));

        while (queue.Count > 0)
        {
            var c = queue.Dequeue();

            var x = c.X1;

            if (Inside(x, c.Y))
            {
                while (Inside(x - 1, c.Y))
                {
                    _outsideLoop[c.Y][x - 1] = true;
                    x--;
                }

                if (x < c.X1)
                {
                    queue.Enqueue(new FillCoordinate(x, c.X1 - 1, c.Y - c.DY, -c.DY));
                }
            }

            while (c.X1 <= c.X2)
            {
                while (Inside(c.X1, c.Y))
                {
                    _outsideLoop[c.Y][c.X1] = true;
                    c.X1++;
                }

                if (c.X1 > x)
                {
                    queue.Enqueue(new FillCoordinate(x, c.X1 - 1, c.Y + c.DY, c.DY));
                }

                if (c.X1 - 1 > c.X2)
                {
                    queue.Enqueue(new FillCoordinate(c.X2 + 1, c.X1 - 1, c.Y - c.DY, -c.DY));
                }

                c.X1++;

                while (c.X1 < c.X2 && !Inside(c.X1, c.Y))
                {
                    c.X1++;
                }

                x = c.X1;
            }
        }
    }

    public bool[][] DumbScaleGrid()
    {
        var newGrid = new bool[_grid.Length * 3][];
        for (var y = 0; y < _grid.Length; y++)
        {
            for (var i = 0; i < 3; i++)
            {
                newGrid[y * 3 + i] = new bool[_grid[y].Length * 3];
            }

            for (var x = 0; x < _grid[y].Length; x++)
            {
                if (_isLoop[y][x])
                {
                    var c = new Coordinate(x, y);
                    var pipeType = c.PipeType;
                    var gridY = y * 3;
                    var gridX = x * 3;

                    if (pipeType == PipeType.Horizontal)
                    {
                        Array.Fill(newGrid[gridY + 1], true, gridX, 3);
                    }
                    else if (pipeType == PipeType.Vertical)
                    {
                        for (var i = 0; i < 3; i++)
                        {
                            newGrid[gridY + i][gridX + 1] = true;
                        }
                    }
                    else if (pipeType == PipeType.Start)
                    {
                        for (var i = 0; i < 3; i++)
                        {
                            Array.Fill(newGrid[gridY + i], true, gridX, 3);
                        }
                    }
                    else
                    {
                        var isNorth = pipeType == PipeType.NorthEast || pipeType == PipeType.NorthWest;
                        var isEast = pipeType == PipeType.NorthEast || pipeType == PipeType.SouthEast;

                        newGrid[gridY + (isNorth ? 0 : 2)][gridX + 1] = true;
                        newGrid[gridY + 1][gridX + (isEast ? 2 : 0)] = true;
                        newGrid[gridY + 1][gridX + 1] = true;
                    }
                }
            }
        }

        return newGrid;
    }


    public void ApplyGrid<T>(T[][] grid, Func<int, int, T> func)
    {
        for (var y = 0; y < grid.Length; y++)
        {
            for (var x = 0; x < grid[y].Length; x++)
            {
                grid[y][x] = func(x, y);
            }
        }
    }

    public IEnumerable<Coordinate> Loop()
    {
        var start = FindStart();
        yield return start;
        var previousPipe = start;
        var nextPipe = start.GetNext(start);
        while (!nextPipe.IsStart)
        {
            yield return nextPipe;
            var oldPipe = nextPipe;
            nextPipe = nextPipe.GetNext(previousPipe);
            previousPipe = oldPipe;
        }
    }

    public void PrintGrid()
    {
        for (var i = 0; i < 3; i++)
        {
            Console.Write("   ");
            for (var x = 0; x < _grid[0].Length; x++)
            {
                Console.Write(x / (int)Math.Pow(10, 2 - i) % 10);
            }

            Console.WriteLine();
        }

        for (var y = 0; y < _grid.Length; y++)
        {
            Console.Write($"{y,2} ");
            foreach (var c in _grid[y].
                         Select((val, x) => new Coordinate(x, y)))
            {
                Console.Write(PipeChars[c.PipeType]);
            }

            Console.WriteLine();
        }
    }

    public void PrintFinalGrid()
    {
        foreach (var row in _grid.Select((r, y) => new { r, y }))
        {
            foreach (var cell in row.r.Select((c, x) => new Coordinate(x, row.y)))
            {
                Console.Write(_isLoop[cell.Y][cell.X]                      ? PipeChars[cell.PipeType] :
                              _insideLoop[cell.Y][cell.X]                  ? "I" :
                              _outsideLoop[cell.Y * 3 + 1][cell.X * 3 + 1] ? "O" : " ");
            }

            Console.WriteLine();
        }
    }

    public void PrintOutsideLoop()
    {
        var maxY = _outsideLoop.Length;
        var maxX = Math.Min(Console.BufferWidth, _outsideLoop[0].Length);

        for (var y = 0; y < maxY; y++)
        {
            for (var x = 0; x < maxX; x++)
            {
                Console.Write(_outsideLoop[y][x] ? "*" : " ");
            }

            Console.WriteLine();
        }
    }

    public record Coordinate
    {
        public Coordinate(int x, int y)
        {
            X = x;
            Y = y;
            IsInsideGrid = X >= 0 && Y >= 0 && X < _grid[0].Length && Y < _grid.Length;
            C = IsInsideGrid ? _grid[Y][X] : ' ';
            IsPipe = IsInsideGrid && Enum.IsDefined(typeof(PipeType), (int)C);
            PipeType = IsPipe ? (PipeType)C : PipeType.None;
            if (PipeType == PipeType.None)
            {
                IsPipe = false;
            }

            IsStart = IsInsideGrid && PipeType == PipeType.Start;
        }

        public int X { get; }
        public int Y { get; }
        public char C { get; }

        public bool IsInsideGrid { get; }
        public bool IsPipe { get; }
        public PipeType PipeType { get; }
        public bool IsStart { get; }

        private IEnumerable<Coordinate> Around =>
            from y in Enumerable.Range(Y - 1, 3)
            from x in Enumerable.Range(X - 1, 3)
            let c = new Coordinate(x, y)
            where c.IsInsideGrid && !(x == X && y == Y)
            select c;

        public IEnumerable<Coordinate> Neighbours => Around.Where(c => c.IsPipe);

        public RelativeDirection RelativeOffset(Coordinate other)
        {
            return (RelativeDirection)(X - other.X + (Y - other.Y) * 3 + 4);
        }

        public bool IsConnected(Coordinate to)
        {
            return IsPipe && to.IsPipe && Around.Contains(to) && ValidDirections[PipeType].
                       Contains(RelativeOffset(to));
        }

        public Coordinate GetNext(Coordinate prev)
        {
            return Neighbours.First(c => c != prev && IsConnected(c));
        }

        public string GetDebug()
        {
            return $"({X,2}, {Y,2}): char: {C} gChar: {_grid[Y][X]} type: {PipeType} isStart: {IsStart} IsPipe: {IsPipe} north: {IsConnected(new Coordinate(X, Y - 1))} west: {IsConnected(new Coordinate(X + 1, Y))} south: {IsConnected(new Coordinate(X, Y + 1))} east: {IsConnected(new Coordinate(X - 1, Y))}";
        }
    }


    private record FillCoordinate(int X1, int X2, int Y, int DY)
    {
        public int X1 { get; set; } = X1;
    }
}