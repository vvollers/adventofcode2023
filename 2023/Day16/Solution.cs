using System;
using System.Collections.Generic;
using System.Linq;
using adventofcode.AdventLib;
using System.Numerics;
using System.Runtime.Intrinsics.X86;
using System.Text;
using AdventOfCode.Y2022.Day08;
using Microsoft.CodeAnalysis.Text;

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

public record Beam
{
    public static readonly Dictionary<Directions, Directions> OppositeDirections = new()
                                                                                   {
                                                                                       { Directions.Up, Directions.Down },
                                                                                       { Directions.Down, Directions.Up },
                                                                                       { Directions.Left, Directions.Right },
                                                                                       { Directions.Right, Directions.Left },
                                                                                   };
    
    private static readonly Dictionary<Directions, Func<Beam, Beam>> MoveActions = new()
                                                                               {
                                                                                   { Directions.Up, beam => new Beam(beam, beam.X, beam.Y-1) },
                                                                                   { Directions.Down, beam => new Beam(beam, beam.X, beam.Y+1) },
                                                                                   { Directions.Left, beam => new Beam(beam, beam.X-1, beam.Y) },
                                                                                   { Directions.Right, beam => new Beam(beam, beam.X+1, beam.Y) },
                                                                               };
    private static readonly Dictionary<Directions, Func<Beam, Beam>> ReverseMoveActions = new()
                                                                               {
                                                                                   { Directions.Up, beam => new Beam(beam, beam.X, beam.Y+1) },
                                                                                   { Directions.Down, beam => new Beam(beam, beam.X, beam.Y-1) },
                                                                                   { Directions.Left, beam => new Beam(beam, beam.X+1, beam.Y) },
                                                                                   { Directions.Right, beam => new Beam(beam, beam.X-1, beam.Y) },
                                                                               };

    private static readonly Dictionary<Directions, Func<Beam, Beam>> TurnLeftActions = new()
                                                                                 {
                                                                                     { Directions.Up, beam => new Beam(beam, Directions.Left) },
                                                                                     { Directions.Down, beam => new Beam(beam, Directions.Right) },
                                                                                     { Directions.Left, beam => new Beam(beam, Directions.Down) },
                                                                                     { Directions.Right, beam => new Beam(beam, Directions.Up) },
                                                                                 };

    private static readonly Dictionary<Directions, Func<Beam, Beam>> TurnRightActions = new()
                                                                                  {
                                                                                      { Directions.Up, beam => new Beam(beam, Directions.Right) },
                                                                                      { Directions.Down, beam => new Beam(beam, Directions.Left) },
                                                                                      { Directions.Left, beam => new Beam(beam, Directions.Up) },
                                                                                      { Directions.Right, beam => new Beam(beam, Directions.Down) },
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
    
    public Beam(Beam otherBeam, int x, int y)
    {
        Direction = otherBeam.Direction;
        X = x;
        Y = y;
    }

    public Directions Direction { get; private init; }

    public int X { get; private init;}
    public int Y { get; private init;}

    public Beam Move() => MoveActions[Direction].Invoke(this);
    public Beam MoveBack() => ReverseMoveActions[Direction].Invoke(this);

    public Beam TurnLeft() => TurnLeftActions[Direction].Invoke(this);

    public Beam TurnRight() => TurnRightActions[Direction].Invoke(this);

    public (Beam, Beam) Split()
    {
        switch (Direction)
        {
            case Directions.Up:
            case Directions.Down:
                return (new Beam(this, Directions.Left), new Beam(this, Directions.Right));
            case Directions.Left:
            case Directions.Right:
                return (new Beam(this, Directions.Up), new Beam(this, Directions.Down));
        }

        return (this, this);
    }
}

[ProblemName("The Floor Will Be Lava")]
class Solution : Solver
{
    public BinaryMap128 Solve(MirrorField input, Beam initialBeam, Dictionary<(int, int, int), BinaryMap128> cache, bool useCache = true)
    {
        var field = new MirrorField(input);

        Queue<(Beam[], Beam)> beams = new Queue<(Beam[], Beam)>();
        beams.Enqueue(([initialBeam], initialBeam));
        
        BinaryMap128 resultMap = new BinaryMap128();
        
        while(beams.Count > 0) {
            var beamCombo = beams.Dequeue();

            var beamList = beamCombo.Item1;
            var beam = beamCombo.Item2;
            var lastBeamList = beamList[^1];
            
            if (cache.TryGetValue((beam.X, beam.Y, (int)lastBeamList.Direction), out var cacheVal))
            {
                resultMap |= cacheVal;
                continue;
            }
            
            beam = beam.Move();
            var isOutside = field.IsOutside(beam.X, beam.Y);

            if (isOutside || field.HasVisited(beam.X, beam.Y, beam.Direction))
            {
                if(useCache && isOutside)
                {
                    for (var idx = beamList.Length - 1; idx >= 0; idx--)
                    {
                        var beamPart = beamList[idx];
                        if (field.IsInside(beamPart.X, beamPart.Y) && field[beamPart.X, beamPart.Y] != '.')
                        {
                            var backMove = beamPart.MoveBack();
                            
                            cache[(beamPart.X, beamPart.Y, (int)backMove.Direction)] = Solve(new MirrorField(field), new Beam(backMove.X, backMove.Y, backMove.Direction), cache, false);
                        }
                    }
                 }
                continue;
            }
            
            field.MarkVisited(beam.X, beam.Y, beam.Direction);

            resultMap[beam.X, beam.Y] = true;

            var fieldChar = field[beam.X, beam.Y];
            Beam newBeam;
            switch (fieldChar)
            {
                case '\\':
                    newBeam = beam.Direction is Directions.Up or Directions.Down ? beam.TurnLeft() : beam.TurnRight();
                    beams.Enqueue(([..beamList, beam], newBeam));
                    continue;
                case '/':
                    newBeam = beam.Direction is Directions.Up or Directions.Down ? beam.TurnRight() : beam.TurnLeft();
                    beams.Enqueue(([..beamList, beam], newBeam));
                    continue;
                case '-':
                    if (beam.Direction is Directions.Up or Directions.Down)
                    {
                        var otherBeams = beam.Split();
                        beams.Enqueue(([..beamList, beam], otherBeams.Item1));
                        beams.Enqueue(([..beamList, beam], otherBeams.Item2));
                    }
                    else
                    {
                        beams.Enqueue(([..beamList, beam], beam));
                    }

                    continue;
                case '|':
                    if (beam.Direction is Directions.Left or Directions.Right)
                    {
                        var otherBeams = beam.Split();
                        beams.Enqueue(([..beamList, beam], otherBeams.Item1));
                        beams.Enqueue(([..beamList, beam], otherBeams.Item2));
                    }
                    else
                    {
                        beams.Enqueue(([..beamList, beam], beam));
                    }
                    continue;
                default:
                    Beam backBeam;
                    while (true)
                    {
                        backBeam = beam;
                        beam = beam.Move();
                        
                        if (field.IsOutside(beam.X, beam.Y) || field.HasVisited(beam.X, beam.Y, beam.Direction))
                        {
                            break;
                        }

                        resultMap[beam.X, beam.Y] = true;
                        
                        if (field[beam.X, beam.Y] != '.')
                        {
                            break;
                        }
                    }

                    beams.Enqueue(([..beamList, backBeam], backBeam));
                    continue;
            }
        }
        return resultMap;
    }

    public object PartOne(string input)
    {
        return Solve(new MirrorField(input.ParseToCharGrid()), new Beam(-1, 0, Directions.Right), new(), false).CountSetBits();
    }
    
    public object PartTwo(string input)
    {
        var field = new MirrorField(input.ParseToCharGrid());
        
        Dictionary<(int, int, int), BinaryMap128> cache = new();

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

        return beams.Select(beam => Solve(field, beam, cache).CountSetBits()).
                     Max();
    }
}