using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using adventofcode.AdventLib;

namespace AdventOfCode.Y2023.Day06;

[ProblemName("Wait For It")]
class Solution : Solver
{
    public object PartOne(string input) =>
        Regex.Matches(input, "\\d+").
              Select(m => long.Parse(m.Value)).
              ToList().
              ZipGrid().
              Select(o => (long)Math.Floor((o.first + Math.Sqrt(o.first * o.first - 4 * o.second)) / 2) - 
                          (long)Math.Ceiling((o.first - Math.Sqrt(o.first * o.first - 4 * o.second)) / 2) + 1).
              Aggregate((long) 1, (acc, i) => acc * i);
    
    public object PartTwo(string input) => PartOne(input.Replace(" ", ""));
}