using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2023.Day07;

[ProblemName("Camel Cards")]
class Solution : Solver
{
    public const string CARDRANKS = "23456789TJQKA";

    public enum HandType
    {
        HighCard = 0,
        Pair = 1,
        TwoPair = 2,
        ThreeOfAKind = 3,
        FullHouse = 4,
        FourOfAKind = 5,
        FiveOfAKind = 6
    }
    
    public record Game(string hand, int score, int bet, int occurring0, int occurring1) : IComparable<Game>
    {
        public HandType type =>
            occurring0 switch
            {
                5 => HandType.FiveOfAKind,
                4 => HandType.FourOfAKind,
                3 => occurring1 == 2 ? HandType.FullHouse : HandType.ThreeOfAKind,
                2 => occurring1 == 2 ? HandType.TwoPair : HandType.Pair,
                _ => HandType.HighCard,
            };
        public int CompareTo(Game other) => type == other.type ? score.CompareTo(other.score) : type.CompareTo(other.type);
    }

    public List<Game> ParseGames(string input, bool jokers = false)
    {
        return input.Trim().Split("\n").Select(line =>
        {
            var parts = line.Split(" ");
            var bet = int.Parse(parts[1]);
            var hand = parts[0];
            
            if (jokers)
            {
                var occuring = hand.GroupBy(c => c)
                                   .ToDictionary(group => group.Key, group => group.Count());
                int score = hand.Aggregate(0, (acc, c) => (acc << 4) + (c == 'J' ? 0 : CARDRANKS.IndexOf(c)+1));
                int jokerCount = 0;
                if (occuring.ContainsKey('J'))
                {
                    jokerCount = occuring['J'];
                    occuring.Remove('J');
                }

                if (occuring.Count == 0)
                {
                    // JJJJJ
                    return new Game(hand, score, bet, 5 ,0);
                }
                
                var maxCount = occuring.Values.Max();
                var maxCountKey = occuring.FirstOrDefault(x => x.Value == maxCount).Key;

                if (jokerCount > 0)
                {
                    occuring[maxCountKey] += jokerCount;
                    maxCount = occuring[maxCountKey];
                }

                var occuring1 = (occuring.Count > 1 ? occuring.OrderByDescending(x => x.Value).Skip(1).First().Value : 0);
                           
                return new Game(hand, score, bet, maxCount, occuring1);
            } else {
                int score = hand.Aggregate(0, (acc, c) => (acc << 4) + CARDRANKS.IndexOf(c));
                var occuring = hand.GroupBy(c => c)
                                    .Select(group => group.Count())
                                    .OrderByDescending(count => count).ToList();
                var occuring1 = (occuring.Count > 1 ? occuring[1] : 0);

                return new Game(hand, score, bet, occuring[0], occuring1);
            }
        }).ToList();
    }
    
    public object PartOne(string input) => ParseGames(input).Order().Select((game, index) => (index+1) * game.bet).Sum();
    
    public object PartTwo(string input) => ParseGames(input, true).Order().Select((game, index) => (index+1) * game.bet).Sum();
}