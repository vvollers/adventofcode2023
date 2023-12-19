using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using adventofcode.AdventLib;
using AdventOfCode.Y2018.Day13;

namespace AdventOfCode.Y2023.Day18;

[ProblemName("Lavaduct Lagoon")]
class Solution : Solver {
    public enum Directions {
        Down = 'D',
        Left = 'L',
        Right = 'R',
        Up = 'U',
    }
    
    public record DigInstruction(Directions Direction, int Distance);

    public record Position(int Row, int Col);
    
    public record Edge(Position Start, Position End);

    public DigInstruction ParseDigInstructions(string line, bool isPartTwo)
    {
        var parts = line.Split(' ');
        var color = parts[2][1..^1];
        Directions direction;
        int distance;

        if (isPartTwo)
        {
            direction = color[^1] switch
                        {
                            '0' => Directions.Right,
                            '1' => Directions.Down,
                            '2' => Directions.Left,
                            '3' => Directions.Up,
                            _   => Directions.Up,
                        };
            distance = int.Parse(color.Substring(1, 5), NumberStyles.HexNumber);
        }
        else
        {
            direction = (Directions)Enum.ToObject(typeof(Directions), parts[0][0]);
            distance = int.Parse(parts[1]);
        }

        return new DigInstruction(direction, distance);
    }
    
    private double TotalEdgeLength(IEnumerable<Edge> edges) => edges.Sum(e => (double)(Math.Abs(e.End.Row - e.Start.Row) + Math.Abs(e.End.Col - e.Start.Col)));
    
    private double Volume(IEnumerable<Edge> edges) => Shoelace(edges) + TotalEdgeLength(edges) / 2 + 1;
    
    private double Shoelace(IEnumerable<Edge> edges) => Math.Abs(edges.Sum(e => (double) e.Start.Row * e.End.Col - (double) e.Start.Col * e.End.Row) / 2);
    
    public IEnumerable<Edge> InstructionsToEdges(IEnumerable<DigInstruction> instructions)
    {
        Position current = new Position(0, 0);
        foreach (var instruction in instructions)
        {
            Position oldCurrent = current;
            
            current = instruction.Direction switch
                      {
                          Directions.Up    => current with { Row = current.Row - instruction.Distance },
                          Directions.Down  => current with { Row = current.Row + instruction.Distance },
                          Directions.Left  => current with { Col = current.Col - instruction.Distance },
                          Directions.Right => current with { Col = current.Col + instruction.Distance },
                          _                => current
                      };
            
            yield return new Edge(oldCurrent, current);
        }
    }
    
    public object PartOne(string input) => 
        Volume(
            InstructionsToEdges(
                input.ParseLineData(line => ParseDigInstructions(line, false))));
    public object PartTwo(string input) => 
        Volume(
            InstructionsToEdges(
                input.ParseLineData(line => ParseDigInstructions(line, true))));
}
