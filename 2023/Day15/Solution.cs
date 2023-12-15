using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using adventofcode.AdventLib;

namespace AdventOfCode.Y2023.Day15;

[ProblemName("Lens Library")]
class Solution : Solver
{
    public int Hash(string part) => part.Aggregate(0, (current, c) => (current + c) * 17 % 256);

    public record LensOperation(string Id, char op, int FocusStrength);

    public LensOperation ParseLensOperation(string input)
    {
        var match = Regex.Match(input, @"(\w+)([-=])(\d*)");
        return new LensOperation(
                match.Groups[1].Value, 
                match.Groups[2].Value[0], 
                string.IsNullOrEmpty(match.Groups[3].Value) ? 0 : int.Parse(match.Groups[3].Value)
            );
    }

    public object PartOne(string input) => input.Split(",").Select(Hash).Sum();

    public object PartTwo(string input)
    {
        var operations = input.Split(",").Select(ParseLensOperation).ToList();
        
        Dictionary<int, List<LensOperation>> boxes = new();

        foreach (var operation in operations)
        {
            var boxNum = Hash(operation.Id);
            
            if (!boxes.ContainsKey(boxNum)) boxes[boxNum] = [];

            if (operation.op == '-')
                boxes[boxNum].RemoveAll(o => o.Id == operation.Id);
            else
                boxes[boxNum].ReplaceOrAdd(o => o.Id == operation.Id, operation);
        }

        return boxes.Select(kv => kv.Value.Select((val, idx) => (1 + kv.Key) * (1 + idx) * val.FocusStrength).Sum()).Sum();
    }
}