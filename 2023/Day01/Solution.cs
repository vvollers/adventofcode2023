using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode.Y2023.Day01;

[ProblemName("Trebuchet?!")]
class Solution : Solver {
    public object PartOne(string input) =>
        input.Split("\n").
              Select(line => 
                         int.Parse(
                             string.Concat(
                                 line.First(char.IsDigit), 
                                 line.Last(char.IsDigit)
                             ))).
              Sum();

    public object PartTwo(string input) => 
        input.Split("\n").
              Select(line => 
                         int.Parse(
                             string.Concat(
                                 Match(line), 
                                 Match(line, RegexOptions.RightToLeft)
                             ))).
              Sum();

    public int Match(string line, RegexOptions options = RegexOptions.None) =>
        new Regex(@"\d|zero|one|two|three|four|five|six|seven|eight|nine", options).Match(line).Value switch
        {
            "zero"  => 0,
            "one"   => 1,
            "two"   => 2,
            "three" => 3,
            "four"  => 4,
            "five"  => 5,
            "six"   => 6,
            "seven" => 7,
            "eight" => 8,
            "nine"  => 9,
            _       => int.Parse(new Regex(@"\d", options).Match(line).Value),
        };
}