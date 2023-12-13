using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2023.Day13;

public static class EnumerableExtensions
{
    public static IEnumerable<IEnumerable<T>> SplitWhen<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        var sublist = new List<T>();
        foreach (var item in source)
        {
            if (predicate(item))
            {
                if (sublist.Any())
                {
                    yield return sublist;
                    sublist = new List<T>();
                }
            }
            else
            {
                sublist.Add(item);
            }
        }

        if (sublist.Any())
        {
            yield return sublist;
        }
    }
}

[ProblemName("Point of Incidence")]
class Solution : Solver
{
    public record Field(char[][] FieldData)
    {
        public int Width => FieldData[0].Length;
        public int Height => FieldData.Length;

        public List<ulong> Horizontal =>
            Enumerable.Range(0, Width).
                       Select(x => Convert.ToUInt64(new string(Enumerable.Range(0, Height).
                                                                          Select(y => FieldData[y][x]).
                                                                          ToArray()), 2)).
                       ToList();

        public List<ulong> Vertical =>
            Enumerable.Range(0, Height).
                       Select(y => Convert.ToUInt64(new string(Enumerable.Range(0, Width).
                                                                          Select(x => FieldData[y][x]).
                                                                          Reverse().
                                                                          ToArray()), 2)).
                       ToList();

        public List<int> HorizontalPalindromes => FindPalindrome(Horizontal);
        public List<int> VerticalPalindromes => FindPalindrome(Vertical);

        public int FirstHorizontalPalindromeIndex => HorizontalPalindromes.Count == 0 ? -1 : HorizontalPalindromes[0];
        public int FirstVerticalPalindromeIndex => VerticalPalindromes.Count == 0 ? -1 : VerticalPalindromes[0];

        public int Score => (FirstVerticalPalindromeIndex + 1) * 100 + FirstHorizontalPalindromeIndex + 1;

        public Field GetAlternative(int x, int y)
        {
            var newFieldData = FieldData.Select(row => row.ToArray()).
                                         ToArray();
            newFieldData[y][x] = newFieldData[y][x] == '0' ? '1' : '0';
            return new Field(newFieldData);
        }

        public bool IsPalindrome(List<ulong> list, int index)
        {
            int minLength = Math.Min(index + 1, list.Count - index - 1);
            return list.Skip(index - minLength + 1).
                        Take(minLength).
                        SequenceEqual(list.Skip(index + 1).
                                           Take(minLength).
                                           Reverse());
        }

        public List<int> FindPalindrome(List<ulong> list) =>
            Enumerable.Range(0, list.Count - 1).
                       Where(i => list[i] == list[i + 1] && IsPalindrome(list, i)).
                       ToList();

        public Field FindAlternativeReflection()
        {
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    var newField = GetAlternative(x, y);

                    var horizontalPalindromes = newField.HorizontalPalindromes;
                    var verticalPalindromes = newField.VerticalPalindromes;

                    if ((horizontalPalindromes.Count > 0 && !Enumerable.SequenceEqual(HorizontalPalindromes, horizontalPalindromes)) ||
                        (verticalPalindromes.Count > 0 && !Enumerable.SequenceEqual(VerticalPalindromes, verticalPalindromes)))
                    {
                        return newField;
                    }
                }
            }

            return this;
        }

        public int GetAlternativeScore(Field original)
        {
            var newHorizontalPalindromes = HorizontalPalindromes.Where(o => !original.HorizontalPalindromes.Contains(o)).
                                                                 ToList();
            var newVerticalPalindromes = VerticalPalindromes.Where(o => !original.VerticalPalindromes.Contains(o)).
                                                             ToList();

            var numCols = newHorizontalPalindromes.Count == 0 ? 0 : newHorizontalPalindromes[0] + 1;
            var numRows = newVerticalPalindromes.Count == 0 ? 0 : newVerticalPalindromes[0] + 1;

            return numRows * 100 + numCols;
        }
    }

    public Field ParseFieldLines(IEnumerable<string> lines) =>
        new(lines.Select(line => line.ToCharArray()).
                  ToArray());

    public List<Field> ParseInput(string input) =>
        input.Replace(".", "0").
              Replace("#", "1").
              Split("\n").
              SplitWhen(line => line.Length == 0).
              Select(ParseFieldLines).
              ToList();

    public object PartOne(string input) =>
        ParseInput(input).
            Sum(field => field.Score);

    public object PartTwo(string input) =>
        ParseInput(input).
            Sum(field => field.FindAlternativeReflection().
                               GetAlternativeScore(field));
}