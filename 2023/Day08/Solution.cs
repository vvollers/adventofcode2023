using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using adventofcode.AdventLib;

namespace AdventOfCode.Y2023.Day08;

[ProblemName("Haunted Wasteland")]
partial class Solution : Solver
{
    private readonly Regex nodeRegex = NodeRegexFunc();

    public record Node(string Id, string Left, string Right);

    private ImmutableDictionary<string, Node> ParseNodes(string input) =>
        input.Split("\n").
              Skip(2).
              Select(line => nodeRegex.Matches(line)).
              Select(parts => new Node(parts[0].Value, 
                                       parts[1].Value, 
                                       parts[2].Value)).
              ToImmutableDictionary(node => node.Id);

    private String ParseInstructions(string input) => input.Split("\n")[0];
    
    public List<string> FindPath(string from,
                                 Func<Node, bool> condition,
                                 ImmutableDictionary<string, Node> nodes,
                                 string instructions)
    {
        var path = new List<string>();
        var current = nodes[from];
        var lrCnt = 0;

        while (!condition(current))
        {
            path.Add(current.Id);
            current = nodes[instructions[lrCnt % instructions.Length] == 'L' ? current.Left : current.Right];
            lrCnt++;
        }

        return path;
    }

    public object PartOne(string input)
    {
        var instructions = ParseInstructions(input);
        var nodes = ParseNodes(input);

        return FindPath("AAA", n => n.Id == "ZZZ", nodes, instructions).Count;
    }
    
    public object PartTwo(string input)
    {
        var instructions = ParseInstructions(input);
        var nodes = ParseNodes(input);

        var deltas = nodes.Values.Where(n => n.Id.EndsWith('A')).
                                 Select(node => FindPath(node.Id, n => n.Id.EndsWith('Z'), nodes, instructions)).
                                 Select(path => (long)path.Count);
        
        return ExtraMath.FindLeastCommonMultiple(deltas);
    }

    [GeneratedRegex("\\w+")]
    private static partial Regex NodeRegexFunc();
}