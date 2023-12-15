using System.Collections.Generic;
using System.Linq;
using adventofcode.AdventLib;

namespace AdventOfCode.Y2023.Day14;

public class Field : CharField
{
    public Field(char[][] fieldData) : base(fieldData)
    {
    }

    public Field UpdateScore()
    {
        Score = 0;
        for (int y = 0; y < FieldData.Length; y++)
        {
            Score += FieldData[y].
                         Count(c => c == 'O') * (FieldData.Length - y);
        }

        return this;
    }

    public int Score { get; private set; }

    public Field FallNorth()
    {
        ApplyToAnyCharacter((c, _, _) => c == 'O', (c, x, y) =>
        {
            var yy = y - 1;
            while (yy >= 0 && FieldData[yy][x] == '.')
            {
                yy--;
            }

            yy++;

            Swap(x, y, x, yy);
        });
       
        return this;
    }

    public Field Cycle()
    {
        for (var i = 0; i < 4; i++)
        {
            FallNorth();
            RotateRight();
        }

        UpdateHash();

        return this;
    }

    public Field Clone() =>
        new(FieldData.Select(row => row.ToArray()).
                      ToArray());
}

[ProblemName("Parabolic Reflector Dish")]
class Solution : Solver
{
    public Field ParseInput(string input)
    {
        return new Field(input.ParseToCharGrid());
    }

    public object PartOne(string input) =>
        ParseInput(input).
            FallNorth().
            UpdateScore().
            Score;

    public object PartTwo(string input)
    {
        var field = ParseInput(input);

        List<Field> history = [field];
        
        var cycleField = field;
        const int maxIterations = 1_000_000_000;
        var index = -1;

        for (var i = 1; i < maxIterations; i++)
        {
            cycleField = cycleField.Clone();
            cycleField.Cycle();

            var localCycleField = cycleField;
            if (history.TryGetIndex(c => c.Hash == localCycleField.Hash, out index))
                break;

            history.Add(cycleField);
        }

        var resultIndex = (maxIterations - index) % (history.Count - index) + index;
        return history[resultIndex].
               UpdateScore().
               Score;
    }
}