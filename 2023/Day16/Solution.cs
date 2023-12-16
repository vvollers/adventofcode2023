using System;
using System.Collections.Generic;
using System.Linq;

using adventofcode.AdventLib;

namespace AdventOfCode.Y2023.Day16;

public class MirrorField : CharField
{
    private readonly Dictionary<(int, int, int), bool> visited = new();

    public MirrorField(char[][] fieldData) : base(fieldData) { }

    public MirrorField(CharField otherMirrorField) : this((char[][])otherMirrorField.FieldData.Clone()) { }

    public bool HasVisited(int x, int y, Directions direction) => visited.ContainsKey((x, y, (int)direction));

    public void MarkVisited(int x, int y, Directions direction)
    {
        visited[(x, y, (int)direction)] = true;
    }

    public int NumVisited => visited.Keys
                                      .Select(key => (key.Item1, key.Item2))
                                      .Distinct()
                                      .Count();
}

public enum Directions
{
    Up,
    Down,
    Left,
    Right,
}

public class Beam
{
    private static readonly Dictionary<Directions, Action<Beam>> MoveActions = new()
                                                                               {
                                                                                   { Directions.Up, beam => beam.Y-- },
                                                                                   { Directions.Down, beam => beam.Y++ },
                                                                                   { Directions.Left, beam => beam.X-- },
                                                                                   { Directions.Right, beam => beam.X++ },
                                                                               };

    private static readonly Dictionary<Directions, Directions> TurnLeftActions = new()
                                                                                 {
                                                                                     { Directions.Up, Directions.Left },
                                                                                     { Directions.Down, Directions.Right },
                                                                                     { Directions.Left, Directions.Down },
                                                                                     { Directions.Right, Directions.Up },
                                                                                 };

    private static readonly Dictionary<Directions, Directions> TurnRightActions = new()
                                                                                  {
                                                                                      { Directions.Up, Directions.Right },
                                                                                      { Directions.Down, Directions.Left },
                                                                                      { Directions.Left, Directions.Up },
                                                                                      { Directions.Right, Directions.Down },
                                                                                  };

    public Beam(int x, int y, Directions direction)
    {
        Direction = direction;
        X = x;
        Y = y;
    }

    public Beam(Beam otherBeam, Directions newDirection)
    {
        Direction = newDirection;
        X = otherBeam.X;
        Y = otherBeam.Y;
    }

    public Directions Direction { get; set; }

    public int X { get; set; }
    public int Y { get; set; }

    public void Move() => MoveActions[Direction].Invoke(this);

    public void TurnLeft() => Direction = TurnLeftActions[Direction];

    public void TurnRight() => Direction = TurnRightActions[Direction];

    public (Beam, Beam) Split()
    {
        switch (Direction)
        {
            case Directions.Up:
            case Directions.Down:
                return (new Beam(this, Direction = Directions.Left), new Beam(this, Direction = Directions.Right));
            case Directions.Left:
            case Directions.Right:
                return (new Beam(this, Direction = Directions.Up), new Beam(this, Direction = Directions.Down));
        }

        return (this, this);
    }
}

[ProblemName("The Floor Will Be Lava")]
class Solution : Solver
{
    public int Solve(MirrorField input, Beam initialBeam)
    {
        var field = new MirrorField(input);

        Queue<Beam> beams = new([initialBeam]);

        var fieldCharActions = new Dictionary<char, Action<Beam>>
                               {
                                   ['\\'] = beam => 
                                            {
                                                if (beam.Direction is Directions.Up or Directions.Down)
                                                    beam.TurnLeft();
                                                else
                                                    beam.TurnRight();
                                                beams.Enqueue(beam);
                                            },
                                   ['/'] = beam => 
                                           {
                                               if (beam.Direction is Directions.Up or Directions.Down)
                                                   beam.TurnRight();
                                               else
                                                   beam.TurnLeft();
                                               beams.Enqueue(beam);
                                           },
                                   ['-'] = beam => 
                                           {
                                               if (beam.Direction is Directions.Up or Directions.Down)
                                               {
                                                   var otherBeams = beam.Split();
                                                   beams.Enqueue(otherBeams.Item1);
                                                   beams.Enqueue(otherBeams.Item2);
                                               }
                                               else
                                                   beams.Enqueue(beam);
                                           },
                                   ['|'] = beam => 
                                           {
                                               if (beam.Direction is Directions.Left or Directions.Right)
                                               {
                                                   var otherBeams = beam.Split();
                                                   beams.Enqueue(otherBeams.Item1);
                                                   beams.Enqueue(otherBeams.Item2);
                                               }
                                               else
                                                   beams.Enqueue(beam);
                                           },
                                   ['.'] = beam => beams.Enqueue(beam)
                               };

        while (beams.Count > 0)
        {
            var beam = beams.Dequeue();
            beam.Move();

            if (field.IsOutside(beam.X, beam.Y) || field.HasVisited(beam.X, beam.Y, beam.Direction))
            {
                continue;
            }

            field.MarkVisited(beam.X, beam.Y, beam.Direction);

            var fieldChar = field[beam.X, beam.Y];

            if (fieldCharActions.ContainsKey(fieldChar))
            {
                fieldCharActions[fieldChar](beam);
            }
        }

        return field.NumVisited;
    }

    public object PartOne(string input) => Solve(new MirrorField(input.ParseToCharGrid()), new Beam(-1, 0, Directions.Right));

    public object PartTwo(string input)
    {
        var field = new MirrorField(input.ParseToCharGrid());

        HashSet<Beam> beams = new();
        for (var x = 0; x < field.Width; x++)
        {
            beams.Add(new Beam(x, -1, Directions.Down));
            beams.Add(new Beam(x, field.Height, Directions.Up));
        }

        for (var y = 0; y < field.Height; y++)
        {
            beams.Add(new Beam(-1, y, Directions.Right));
            beams.Add(new Beam(field.Width, y, Directions.Left));
        }

        return beams.Select(beam => Solve(field, beam)).
                     Max();
    }
}