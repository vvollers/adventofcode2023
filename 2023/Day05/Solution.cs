using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode.Y2023.Day05;

[ProblemName("If You Give A Seed A Fertilizer")]
partial class Solution : Solver
{
    private static readonly Regex SeedLineRegex = SeedLineRegexFunc();
    private static readonly Regex MapLineRegex = MapLineRegexFunc();
    
    public record Range(long from, long to);
    public record Mapping(string from, string to, long source, long destination, long range);

    private List<long> GetSeeds(string input)
    {
        var match = SeedLineRegex.Match(input);
        return match.Groups[2].Captures.Select(x => long.Parse(x.Value)).ToList();
    }

    private List<Mapping> GetMapping(string input)
    {
        var match = MapLineRegex.Matches(input);
        var mappings = new List<Mapping>();
        foreach (Match m in match)
        {
            var from = m.Groups[1].Value;
            var to = m.Groups[2].Value;

            var numMaps = m.Groups[3].Captures.Count;
            for (var i = 0; i < numMaps; i++)
            {
                var destination = long.Parse(m.Groups[4].Captures[i].Value);
                var source = long.Parse(m.Groups[5].Captures[i].Value);
                var range = long.Parse(m.Groups[6].Captures[i].Value);
                mappings.Add(new Mapping(from, to, source, destination, range));
            }
        }

        mappings = mappings.OrderBy(obj => obj.from)
            .ThenBy(obj => obj.source).ToList();

        return mappings;
    }

    public long Map(List<Mapping> mappings, string from, string to, long number)
    {
        var map = mappings.FirstOrDefault(x =>
            x.from == from && x.to == to && number >= x.source && number <= x.source + x.range);
        if (map == null)
        {
            return number;
        }

        var diff = number - map.source;
        return map.destination + diff;
    }

    private long FindLocation(long seedNum, List<Mapping> mappings, Dictionary<string, string> conversions)
    {
        var current = "seed";
        var destination = "location";
        var currentNum = seedNum;

        while (current != destination)
        {
            currentNum = Map(mappings, current, conversions[current], currentNum);

            if (currentNum == -1)
            {
                return long.MaxValue;
            }

            current = conversions[current];
        }

        return currentNum;
    }
    
    public List<Range> GetRanges(List<Range> ranges, List<Mapping> maps, String part)
    {
        var relevantMappings = maps.Where(o => o.from == part).ToList();
        var newRanges = new List<Range>();
        foreach (var r in ranges)
        {
            var range = r;
            foreach (var mapping in relevantMappings)
            {
                var mapTo = mapping.source + mapping.range - 1;
                if (range.from < mapping.source)
                {
                    newRanges.Add(range with { to = Math.Min(range.to, mapping.source - 1) });
                    range = range with { from = mapping.source };
                    if (range.from > range.to)
                        break;
                }

                if (range.from <= mapTo)
                {
                    newRanges.Add(new Range(range.from + (mapping.destination - mapping.source), Math.Min(range.to, mapTo) + (mapping.destination - mapping.source)));
                    range = range with { from = mapTo + 1 };
                    if (range.from > range.to)
                        break;
                }
            }
            if (range.from <= range.to) {
                newRanges.Add(range);
            }
        }

        return newRanges;
    }
    
    public object PartOne(string input)
    {
        var seeds = GetSeeds(input);
        var mappings = GetMapping(input);

        Dictionary<string, string> conversions = new();
        foreach (var m in mappings)
        {
            conversions[m.from] = m.to;
        }

        return seeds.Select(o => FindLocation(o, mappings, conversions)).Min();
    }
    
    public object PartTwo(string input)
    {
       var seeds = GetSeeds(input);
       var mapping = GetMapping(input);
       
       var ranges = seeds.Chunk(2).Select(o => new Range(o[0], o[0]+o[1]-1)).ToList();
       
       string[] mapOrder = ["seed", "soil", "fertilizer", "water", "light", "temperature", "humidity"];
       
       // linearly map the ranges
       // since going from layer to layer without adjustment just means the value in the next layer
       // is the same as this one, we can just map the ranges in order and apply the mapping when neccessary
       // this way we just keep track of one set of "adjustments" which we change as we go through the layers
       // then finally we just find the minimum value in the range
       foreach(var map in mapOrder)
       {
           ranges = GetRanges(ranges, mapping, map);
       }
       
       return ranges.Min(r => r.from);
    }

    [GeneratedRegex(@"seeds: ((\d+)\s?)+")]
    private static partial Regex SeedLineRegexFunc();
    
    [GeneratedRegex(@"(\w+)-to-(\w+) map:\n((\d+) (\d+) (\d+)\n)+")]
    private static partial Regex MapLineRegexFunc();
}