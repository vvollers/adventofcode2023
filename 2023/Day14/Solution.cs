using System.Collections.Generic;
using System.Linq;
using adventofcode.AdventLib;

namespace AdventOfCode.Y2023.Day14;

public enum Orientation
{
    NORTH,
    EAST,
    SOUTH,
    WEST
}

public class Field : CharField
{
    
    
    public Field(char[][] fieldData) : base(fieldData)
    {
        
    }

    public Orientation Orientation { get; set; } = Orientation.NORTH;
    
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
            switch (Orientation)
            {
                case Orientation.NORTH:
                    var yy = y - 1;
                    while (yy >= 0 && FieldData[yy][x] == '.')
                    {
                        yy--;
                    }

                    yy++;

                    Swap(x, y, x, yy);
                    break;
                case Orientation.WEST:
                    var xx = x - 1;
                    while (xx >= 0 && FieldData[y][xx] == '.')
                    {
                        xx--;
                    }

                    xx++;

                    Swap(x, y, xx, y);
                    break;
                case Orientation.SOUTH:
                    var yy2 = y + 1;
                    while (yy2 < Height && FieldData[yy2][x] == '.')
                    {
                        yy2++;
                    }

                    yy2--;

                    Swap(x, y, x, yy2);
                    break;
                case Orientation.EAST:
                    var xx2 = x + 1;
                    while (xx2 < Width && FieldData[y][xx2] == '.')
                    {
                        xx2++;
                    }

                    xx2--;

                    Swap(x, y, xx2, y);
                    break;
            }
        });
       
        return this;
    }

    public Field Cycle()
    {
        for (var i = 0; i < 4; i++)
        { 
            FallNorth();
           // RotateRight();
           Orientation++;
           if(Orientation > Orientation.WEST) {
               Orientation = Orientation.NORTH;
           }
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

        //List<Field> history = [field];
        Dictionary<ulong, int> scoreHistory = new();
        List<ulong> history = new();
        
        history.Add(field.Hash);
        scoreHistory[field.Hash] = field.Score;
        
        //var cycleField = field;
        const int maxIterations = 1_000_000_000;
        var index = -1;

        for (var i = 1; i < maxIterations; i++)
        {
           // cycleField = cycleField.Clone();
           // cycleField.Cycle();
           field.Cycle();

            //var localCycleField = cycleField;
            if (history.TryGetIndex(c => c == field.Hash, out index))
                break;

            field.UpdateScore();
            history.Add(field.Hash);
            scoreHistory[field.Hash] = field.Score;
        }

        var resultIndex = (maxIterations - index) % (history.Count - index) + index;
        var hash = history[resultIndex];
        return scoreHistory[hash];
    }
}