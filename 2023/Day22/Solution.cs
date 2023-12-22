using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using adventofcode.AdventLib;

namespace AdventOfCode.Y2023.Day22;

[ProblemName("Sand Slabs")]
class Solution : Solver
{
    public int _blocks = -1;
    public int _blocksCounter2 = 0;
    public static Vector3 Down = new (0, 0, -1);
    
    public record Block(Vector3 minPoint, Vector3 maxPoint, string name)
    {
        public int[] Xs => Enumerable.Range((int) minPoint.X, (int) (maxPoint.X - minPoint.X + 1)).ToArray();
        public int[] Ys => Enumerable.Range((int) minPoint.Y, (int) (maxPoint.Y - minPoint.Y + 1)).ToArray();
        public int[] Zs => Enumerable.Range((int) minPoint.Z, (int) (maxPoint.Z - minPoint.Z + 1)).ToArray();
        
        public List<Block> Above { get; set; } = new();
        public List<Block> Supporters { get; set; } = new();
    }

    public Block ParseBlockLine(string line)
    {
        var blockEnds = line.Split('~');
        var fromCoords = blockEnds[0].
                         Split(',').
                         Select(int.Parse).ToArray();
        var toCoords = blockEnds[1].
                       Split(',').
                       Select(int.Parse).
                       ToArray();
        _blocks++;
        return new Block(new Vector3(fromCoords[0], fromCoords[1], fromCoords[2]),
                         new Vector3(toCoords[0], toCoords[1], toCoords[2]), "" + (char)('A' + _blocks));
    }

    public void MakeBlockFallOne(string name, List<Block> blocks)
    {
        if (blocks.TryGetIndex(block => block.name == name, out var idx))
        {
            var block = blocks[idx];
            blocks[idx] = new Block(block.minPoint + Down, block.maxPoint + Down, block.name);
        }
    }
    
    public static bool DoBlocksIntersect(Block a, Block b)
    {
        if (a.maxPoint.X < b.minPoint.X || b.maxPoint.X < a.minPoint.X)
            return false;

        if (a.maxPoint.Y < b.minPoint.Y || b.maxPoint.Y < a.minPoint.Y)
            return false;

        if (a.maxPoint.Z < b.minPoint.Z || b.maxPoint.Z < a.minPoint.Z)
            return false;

        return true; 
    }
    
    public bool CanBlockFallOne(string name, List<Block> blocks) {
        if (blocks.TryGetIndex(block => block.name == name, out var idx))
        {
            var block = blocks[idx];
            
            if (block.minPoint.Z <= 1)
            {
                return false;
            }
            
            var blockOneDown = block with
                               {
                                   minPoint = block.minPoint + Down,
                                   maxPoint = block.maxPoint + Down,
                               };
            
            var blocksOnThisLevel = blocks.Where(b => blockOneDown.minPoint.Z >= b.minPoint.Z && blockOneDown.minPoint.Z <= b.maxPoint.Z).ToList();
            
            return !blocksOnThisLevel.Any(otherBlock => DoBlocksIntersect(blockOneDown, otherBlock));
        }

        return false;
    }
    
    public bool CanBlockFallOne(string name, List<Block> blocks, string blockToIgnore) {
        if (blocks.TryGetIndex(block => block.name == name, out var idx))
        {
            var block = blocks[idx];
            
            if (block.minPoint.Z <= 1)
            {
                return false;
            }
            
            var blockOneDown = block with
                               {
                                   minPoint = block.minPoint + Down,
                                   maxPoint = block.maxPoint + Down,
                               };
            
            var blocksOnThisLevel = blocks.Where(b => b.name != blockToIgnore && blockOneDown.minPoint.Z >= b.minPoint.Z && blockOneDown.minPoint.Z <= b.maxPoint.Z).ToList();
            
            return !blocksOnThisLevel.Any(otherBlock => DoBlocksIntersect(blockOneDown, otherBlock));
        }

        return false;
    }
    
    public List<Block> GetBlocksBeingSupportedBy(Block block, List<Block> blocks)
    {
        var blockOneUp = block with
                           {
                               minPoint = block.minPoint with { Z = block.maxPoint.Z + 1 },
                               maxPoint = block.maxPoint with { Z = block.maxPoint.Z + 1 },
                           };
            
        var blocksOnThisLevel = blocks.Where(b => blockOneUp.minPoint.Z >= b.minPoint.Z && blockOneUp.minPoint.Z <= b.maxPoint.Z).ToList();
            
        return blocksOnThisLevel.Where(otherBlock => DoBlocksIntersect(blockOneUp, otherBlock)).ToList();
    }

    public bool CanBlockBeRemoved(string name, List<Block> blocks)
    {
        foreach (var block in blocks)
        {
            if (block.name != name && CanBlockFallOne(block.name, blocks, name))
            {
                return false;
            }
        }

        return true;
    }
    
    public int BlocksWhichWouldFall(string name, List<Block> blocks)
    {
        var numFallen = 1;
        HashSet<string> blocksHit = new();
        var oldBlocks = new List<Block>(blocks.Where(block => block.name != name));
        while (numFallen > 0)
        {
            numFallen = 0;
            var blockWorkingSet = new List<Block>(oldBlocks);
            foreach (var block in blockWorkingSet)
            {
                if (CanBlockFallOne(block.name, oldBlocks))
                {
                    blocksHit.Add(block.name);
                    MakeBlockFallOne(block.name, oldBlocks);
                    numFallen++;
                }
            }
        }

        return blocksHit.Count;
    }

    public List<Block> FallBlocks(List<Block> blocks)
    {
        List<Block> newBlocks = new List<Block>(blocks);

        var numFallen = 1;
        while (numFallen > 0)
        {
            numFallen = 0;
            for (int i = 0; i < blocks.Count; i++)
            {
                if (CanBlockFallOne(blocks[i].name, newBlocks)) {
                    newBlocks[i] = new Block(newBlocks[i].minPoint + Down, newBlocks[i].maxPoint + Down, newBlocks[i].name);
                    numFallen++;
                }
            }
        }

        return newBlocks;
    }

    public List<Block> PrepareBlockList(List<Block> blocks)
    {
        blocks = blocks.OrderBy(block => block.minPoint.Z).ToList();
        foreach (var block in blocks)
        {
            block.Above = GetBlocksBeingSupportedBy(block, blocks);
            foreach(var aboveBlock in block.Above)
            {
                aboveBlock.Supporters.Add(block);
            }
        }
        return blocks;
    }
    
    public object PartOne(string input)
    {
        var blocks = input.ParseLineData(ParseBlockLine);
        blocks = PrepareBlockList(blocks);
        blocks = FallBlocks(blocks);

        return blocks.
               AsParallel().
               Count(block => CanBlockBeRemoved(block.name, blocks));
    }

    public object PartTwo(string input) {
        var blocks = input.ParseLineData(ParseBlockLine);
        
        blocks = PrepareBlockList(blocks);
        blocks = FallBlocks(blocks);
        
        return blocks.AsParallel().Sum(block => BlocksWhichWouldFall(block.name, blocks));
    }
}
