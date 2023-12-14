using System;
using System.Linq;
using AngleSharp.Html.Dom.Events;

namespace adventofcode.AdventLib;

public class CharField :IEquatable<CharField>
{
    public CharField(char[][] fieldData)
    {
        FieldData = fieldData;
        UpdateHash();
    }

    protected void UpdateHash()
    {
        unchecked
        {
            ulong hash = 5381;
            int y = 0;
            
            foreach (char[] array in FieldData)
            {
                int x = 0;
                foreach (char c in array)
                {
                    x++;
                    hash = ((hash << 5) + hash) + c;
                }

                y++;
            }

            Hash = hash;
        }
    }

    public void ApplyToAll(Action<char, int, int> action)
    {
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                action(FieldData[y][x], x, y);
            }
        }
    }
    
    public void ApplyToAll(Action<char> action)
    {
        ApplyToAll((c, _, _) => action(c));
    }

    public void ApplyToAnyCharacter(Func<char, int, int, bool> match, Action<char, int, int> action)
    {
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                if(match(FieldData[y][x],x, y)) {
                    action(FieldData[y][x], x, y);
                }
            }
        }
    }
    
    public void Swap(int x1, int y1, int x2, int y2) =>
        (FieldData[y1][x1], FieldData[y2][x2]) = (FieldData[y2][x2], FieldData[y1][x1]);
    
    public void RotateRight()
    {
        FieldData = Rotate90Clockwise(FieldData);
    }
    
    public static char[][] Rotate90Clockwise(char[][] array)
    {
        int n = array.Length;
        char[][] result = new char[n][];

        for (int i = 0; i < n; i++)
        {
            result[i] = new char[n];
        }

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                result[j][n - i - 1] = array[i][j];
            }
        }

        return result;
    }
    
    public char[][] FieldData { get; private set; }
    
    public int Width => FieldData[0].Length;
    public int Height => FieldData.Length;
    
    public ulong Hash { get; set; }

    public bool Equals(CharField other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Hash == other.Hash;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != this.GetType())
        {
            return false;
        }

        return Equals((CharField)obj);
    }

    public override int GetHashCode()
    {
        return Hash.GetHashCode();
    }
    
    public override string ToString()
    {
        var lines = "";
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                lines += FieldData[y][x];
            }

            lines += "\n";
        }

        lines += $"Width: {Width}, Height: {Height}, Hash: " + string.Format("0x{0:X8}", Hash);

        return lines;
    }
}