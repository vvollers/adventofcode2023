using System;
using System.Collections.Generic;
using System.Linq;
using adventofcode.AdventLib;

namespace AdventOfCode.Y2023.Day19;

public enum Operator
{
    GreaterThan = '>',
    LessThan = '<',
    None,
}
public record Part(Dictionary<string, int> Properties)
{
    public override string ToString()
    {
        return $"Part ({string.Join(", ", Properties.Select(kv => $"{kv.Key}={kv.Value}"))})";
    }

    public int Score =>
        Properties.Select(kv => kv.Value).
                   Sum();
};
public record RulePart(string Name, Operator Operator, int Value, string NextRule);
public record Rule(string Name, List<RulePart> RuleParts, string DefaultRule) {
    public override string ToString()
    {
        return $"Rule '{Name}' ({string.Join(", ", RuleParts)}) [Default: {DefaultRule}";
    }
};

[ProblemName("Aplenty")]
class Solution : Solver
{
    private Operator GetFirstMatchingOperator(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return Operator.None;
        }

        foreach (var c in input.ToUpper().Where(c => Enum.IsDefined(typeof(Operator), (int)c)))
        {
            return (Operator)c;
        }

        return Operator.None;
    }

    private Part ParsePartLine(string line)
    {
        return new Part(line[1..^1].Split(',').
                           Select(x => x.Split('=')).
                           ToDictionary( k=> k[0], v => int.Parse(v[1])));
    }

    private RulePart ParseRulePart(string rule)
    {
        var op = GetFirstMatchingOperator(rule);
        if (op == Operator.None)
        {
            return new RulePart("", Operator.None, 0, rule);
        }

        var kv = rule.Split(':');
        var rp = kv[0].Split((char)op);
        return new RulePart(rp[0], op, int.Parse(rp[1]), kv[1]);
    }

    private Rule ParseRuleLine(string line)
    {
        var kv = line[..^1].Split('{');
        var name = kv[0];
        var defaultRule = "";
        List<RulePart> ruleParts = [];
        foreach (var part in kv[1].
                             Split(',').
                             Select(ParseRulePart))
        {
            if (part.Operator == Operator.None)
            {
                defaultRule = part.NextRule;
            }
            else
            {
                ruleParts.Add(part);
            }
        }
        
        return new Rule(name, ruleParts, defaultRule);
    }

    private bool IsAccepted(string currentRule,
                            IReadOnlyDictionary<string, int> partProps,
                            IReadOnlyDictionary<string, Rule> rules)
    {
        var rule = rules[currentRule];
        var hasRuleMatch = rule.RuleParts.TryGetIndex(
                rp => 
                    (rp.Operator == Operator.GreaterThan && partProps[rp.Name] > rp.Value) || 
                    (rp.Operator == Operator.LessThan && partProps[rp.Name] < rp.Value), 
                out var index);
        var nextRule = !hasRuleMatch ? rule.DefaultRule : rule.RuleParts[index].NextRule;
        return nextRule switch
               {
                   "A" => true,
                   "R" => false,
                   _   => IsAccepted(nextRule, partProps, rules),
               };
    }

    private IEnumerable<Dictionary<string, Range>> GetAccepted(string currentRule, Dictionary<string, Range> partRanges, Dictionary<string, Rule> rules)
    {
        var rule = rules[currentRule];

        List<Dictionary<string, Range>> options = new();
        var currentPartRanges = partRanges.ToDictionary(kv => kv.Key, kv => kv.Value);
        foreach (var rulePart in rule.RuleParts)
        {
            var matchingRange = currentPartRanges[rulePart.Name];
            // split into matching and non matching range
            var (matching, nonMatching) = rulePart.Operator == Operator.GreaterThan ? matchingRange.SplitGreaterThan(rulePart.Value) : matchingRange.SplitLessThan(rulePart.Value);
            // continue with non matching
            var matchingRanges = currentPartRanges.ToDictionary(kv => kv.Key, kv => kv.Value);
            matchingRanges[rulePart.Name] = matching;
            currentPartRanges[rulePart.Name] = nonMatching;

            // add matching (and rest of current set) to options
            if (rulePart.NextRule == "A")
            {
                options.Add(matchingRanges);
            } else if (rulePart.NextRule != "R")
            {
                options.AddRange(GetAccepted(rulePart.NextRule, matchingRanges, rules));
            }
        }
        if (rule.DefaultRule == "A")
        {
            options.Add(currentPartRanges);
        } else if (rule.DefaultRule != "R") {
            options.AddRange(GetAccepted(rule.DefaultRule, currentPartRanges, rules));
        }

        return options;
    }
    
    public record Range(int Min, int Max)
    {
        public (Range matching, Range nonMatching) SplitLessThan(int value) => (this with { Max = value-1 }, this with { Min = value });
        public (Range matching, Range nonMatching) SplitGreaterThan(int value) => (this with { Min = value+1 }, this with { Max = value });
        public long Length => Max - Min + 1;
    }
    
    public object PartOne(string input)
    {
        (var rules, var parts) = input.ParseLineData(ParseRuleLine, ParsePartLine);

        var ruleDict = rules.ToDictionary(k => k.Name, k => k);

        return parts.Where(p => IsAccepted("in", p.Properties, ruleDict)).Select(p => p.Score).
                        Sum();
    }

    public object PartTwo(string input) {
        (var rules, _) = input.ParseLineData(ParseRuleLine, ParsePartLine);

        var ruleDict = rules.ToDictionary(k => k.Name, k => k);

        string[] partTypes = ["x", "m", "a", "s"];
        var ranges = partTypes.ToDictionary(o => o, _ => new Range(1,4000));
        
        return GetAccepted("in", ranges, ruleDict).Sum(combination => partTypes.Select(p => combination[p].Length).Aggregate((a, b) => a * b));
    }
}
