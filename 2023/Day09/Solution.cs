using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2023.Day09;

[ProblemName("Mirage Maintenance")]
class Solution : Solver
{
    public IEnumerable<long> ZipDiff(IEnumerable<long> numbers) => numbers.Zip(numbers.Skip(1), (a, b) => b - a);

    long RecurseRight(List<long> numbers) => numbers.Count == 0 ? 0 : RecurseRight(ZipDiff(numbers).ToList()) + numbers.Last();
    
    long RecurseLeft(List<long> numbers) => numbers.Count == 0 ? 0 : numbers.First() - RecurseLeft(ZipDiff(numbers).ToList());
    
    List<List<long>> ParseNumbers(string input) => input.Split("\n").
                                                         Select(line => line.Split(" ").
                                                                             Select(long.Parse).
                                                                             ToList()).
                                                         ToList();
    public object PartOne(string input) =>
        ParseNumbers(input).
            Select(RecurseRight).
            Sum();
   

    public object PartTwo(string input) =>
        ParseNumbers(input).
            Select(RecurseLeft).
            Sum();
}