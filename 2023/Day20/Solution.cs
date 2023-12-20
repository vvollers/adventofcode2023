using System;
using System.Collections.Generic;
using System.Linq;
using adventofcode.AdventLib;

namespace AdventOfCode.Y2023.Day20;

public enum ModuleType
{
    Broadcaster,
    Sink,
    FlipFlop = '%',
    Conjunction = '&',
}

public record Module(string Name, ModuleType ModuleType, List<string> Outputs)
{
    public override string ToString()
    {
        return $"{ModuleType} {Name} -> {string.Join(", ", Outputs)}";
    }
}

public record Pulse(bool High, string Source, string Destination)
{
    public override string ToString()
    {
        return $"{Source} -[{(High ? "high" : "low")}]-> {Destination}";
    }
}

public class StateMachineNode
{
    public bool CurrentState { get; set; } = false;
    public Dictionary<string, bool> State { get; set; } = new();
    public List<StateMachineNode> Outputs { get; set; } = new();
    public List<StateMachineNode> Inputs { get; set; } = new();
    public long NumPulsesReceived { get; set; } = 0;
    public Module Module { get; set; }

    public StateMachineNode(Module module)
    {
        Module = module;
    }

    public int DistanceTo(StateMachineNode other)
    {
        var visited = new HashSet<StateMachineNode>();
        var queue = new Queue<(StateMachineNode, int)>();
        queue.Enqueue((this, 0));
        while (queue.TryDequeue(out var current))
        {
            if (visited.Contains(current.Item1))
            {
                continue;
            }

            visited.Add(current.Item1);
            if (current.Item1 == other)
            {
                return current.Item2;
            }

            foreach (var output in current.Item1.Inputs.Where(o => o.Module.ModuleType is ModuleType.FlipFlop or ModuleType.Broadcaster))
            {
                queue.Enqueue((output, current.Item2 + 1));
            }
        }

        return -1;
    }

    public void AddOutputNode(StateMachineNode output)
    {
        Outputs.Add(output);
    }

    public void AddInputNode(StateMachineNode input)
    {
        State[input.Module.Name] = false;
        Inputs.Add(input);
    }

    public List<Pulse> Receive(bool highPulse, string state)
    {
        NumPulsesReceived++;
        var pulses = new List<Pulse>();
        if (Module.ModuleType == ModuleType.FlipFlop)
        {
            if (!highPulse)
            {
                CurrentState = !CurrentState;
                pulses.AddRange(Outputs.Select(o => new Pulse(CurrentState, Module.Name, o.Module.Name)));
            }
        }
        else if (Module.ModuleType == ModuleType.Conjunction)
        {
            State[state] = highPulse;
            CurrentState = !State.All(kv => kv.Value);

            pulses.AddRange(Outputs.Select(o => new Pulse(CurrentState, Module.Name, o.Module.Name)));
        }
        else if (Module.ModuleType == ModuleType.Broadcaster)
        {
            CurrentState = highPulse;
            pulses.AddRange(Outputs.Select(o => new Pulse(CurrentState, Module.Name, o.Module.Name)));
        }
        else
        {
            CurrentState = highPulse;
        }

        return pulses;
    }

    public override string ToString()
    {
        var outputs = Outputs.Select(o => o.Module.Name).
                              ToList();
        return $"{Module.Name} -> {string.Join(", ", outputs)} |";
    }
}

[ProblemName("Pulse Propagation")]
class Solution : Solver
{
    public Module ParseModule(string line)
    {
        var moduleType = Parsing.FromChar(line[0], ModuleType.Broadcaster);
        line = line.Replace(" ", "");
        var parts = line.Split("->");
        var name = parts[0];
        if (moduleType != ModuleType.Broadcaster)
        {
            name = name[1..];
        }

        var outputs = parts[1].
                      Split(",").
                      ToList();
        return new Module(name, moduleType, outputs);
    }

    public (long, long) PressButton(Dictionary<string, StateMachineNode> stateMachine)
    {
        Queue<Pulse> pulses = new();
        pulses.Enqueue(new Pulse(false, "button", "broadcaster"));
        var numLow = 0;
        var numHigh = 0;
        foreach (var kv in stateMachine)
        {
            kv.Value.NumPulsesReceived = 0;
        }

        while (pulses.TryDequeue(out var pulse))
        {
            if (pulse.High)
            {
                numHigh++;
            }
            else
            {
                numLow++;
            }

            var node = stateMachine[pulse.Destination];
            var newPulses = node.Receive(pulse.High, pulse.Source);

            foreach (var p in newPulses)
            {
                pulses.Enqueue(p);
            }
        }

        return (numLow, numHigh);
    }

    public Dictionary<string, StateMachineNode> BuildStateMachine(List<Module> modules)
    {
        Dictionary<string, StateMachineNode> stateMachine = modules.ToDictionary(o => o.Name, o => new StateMachineNode(o));

        stateMachine["rx"] = new StateMachineNode(new Module("rx", ModuleType.Sink, new List<string>()));
        foreach (var module in modules)
        {
            var node = stateMachine[module.Name];
            foreach (var outputName in module.Outputs)
            {
                var otherNode = stateMachine[outputName];
                node.AddOutputNode(otherNode);
                otherNode.AddInputNode(node);
            }
        }

        return stateMachine;
    }

    public object PartOne(string input)
    {
        /*    input = """
                    broadcaster -> a
                    %a -> inv, con
                    &inv -> b
                    %b -> con
                    &con -> output
                    """;*/
        var modules = input.ParseLineData(ParseModule);

        var stateMachine = BuildStateMachine(modules);

        long totalLow = 0;
        long totalHigh = 0;
        for (var i = 0; i < 1000; i++)
        {
            var (nLow, nHigh) = PressButton(stateMachine);
            totalLow += nLow;
            totalHigh += nHigh;
        }

        Console.WriteLine($"totalLow: {totalLow}, totalHigh: {totalHigh}");

        return totalLow * totalHigh;
    }

    public void BuildStateMachinePartUntil(string name, Dictionary<string, StateMachineNode> originalStateMachine, Dictionary<string, StateMachineNode> newStateMachine)
    {
        var current = originalStateMachine[name];
        var newNode = new StateMachineNode(current.Module);
        newStateMachine[name] = newNode;
        foreach (var parent in current.Inputs)
        {
            if (!newStateMachine.ContainsKey(parent.Module.Name))
            {
                BuildStateMachinePartUntil(parent.Module.Name, originalStateMachine, newStateMachine);
            }

            var newParent = newStateMachine[parent.Module.Name];
            newNode.AddInputNode(newParent);
            newParent.AddOutputNode(newNode);
        }
    }

    public object PartTwo(string input)
    {
        var modules = input.ParseLineData(ParseModule);

        var stateMachine = BuildStateMachine(modules);

        var rxNode = stateMachine["rx"];

        string[] cycleInputNodes = rxNode.Inputs.First().
                                          Inputs.Select(i => i.Module.Name).
                                          ToArray();

        Dictionary<string, Dictionary<string, StateMachineNode>> partialStateMachines = cycleInputNodes.ToDictionary(n => n, n => new Dictionary<string, StateMachineNode>());
        Dictionary<string, long> numPressesForInput = cycleInputNodes.ToDictionary(n => n, n => 0L);

        foreach (var name in cycleInputNodes)
        {
            BuildStateMachinePartUntil(name, stateMachine, partialStateMachines[name]);
            Console.WriteLine("State machine Graphviz for " + name + ":");
            foreach (var kv in partialStateMachines[name])
            {
                if (kv.Value.Module.ModuleType == ModuleType.Conjunction)
                {
                    Console.WriteLine($"{kv.Key} [fillcolor=blue, shape=\"hexagon\", style=\"filled\", labelfontcolor=\"white\"];");
                }
            }

            foreach (var kv in partialStateMachines[name])
            {
                if (kv.Value.Outputs.Count == 0)
                {
                    continue;
                }

                Console.Write(kv.Key + " -> " + String.Join(", ", kv.Value.Outputs.Select(o => o.Module.Name).
                                                                     ToList()));
                if (kv.Value.Module.ModuleType == ModuleType.Conjunction)
                {
                    Console.Write(" [color=blue]");
                }

                Console.WriteLine();
            }

            Console.WriteLine();
            var orderedList = partialStateMachines[name].
                              Where(kv => kv.Value.Module.ModuleType == ModuleType.FlipFlop).
                              OrderBy(kv => kv.Value.DistanceTo(partialStateMachines[name]["broadcaster"]));

            var nodeWithMostOutputs = partialStateMachines[name].
                                      Select(kv => kv.Value).
                                      OrderByDescending(node => node.Outputs.Count).
                                      First();
            var broadcasterNode = partialStateMachines[name]["broadcaster"];

            String bitMap = "";
            foreach (var kv in orderedList)
            {
                bool bitIsSet = !(kv.Value.Inputs.Contains(nodeWithMostOutputs) && !kv.Value.Inputs.Contains(broadcasterNode));
                bitMap = (bitIsSet ? "1" : "0") + bitMap;
            }

            Console.WriteLine("Bit map for " + name + ": " + bitMap);
            numPressesForInput[name] = Convert.ToInt32(bitMap, 2);
        }

        return ExtraMath.FindLeastCommonMultiple(numPressesForInput.Select(o => o.Value));
    }
}