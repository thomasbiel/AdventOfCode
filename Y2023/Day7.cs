using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2023;

public class Day7 : Day
{
    private const char Joker = 'J';
    
    private enum Mode
    {
        Classic,
        Joker
    }
    
    private readonly struct Card : IComparable<Card>, IEquatable<Card>
    {
        public Card(char name, int value)
        {
            this.Name = name;
            this.Value = value;
        }
        
        public char Name { get; }
        
        public int Value { get; }

        public int CompareTo(Card other) => this.Value.CompareTo(other.Value);

        public bool Equals(Card other) => this.Value == other.Value;

        public override bool Equals(object obj) => obj is Card c && this.Equals(c);

        public override int GetHashCode() => this.Value;

        public override string ToString() => this.Name.ToString();
    }

    private class Hand : IComparable<Hand>, IEquatable<Hand>
    {
        private readonly Card[] cards;
        
        public Hand(string cardNames, IReadOnlyDictionary<char, Card> cardValues, Mode mode)
        {
            this.cards = cardNames.Select(c => cardValues[c]).ToArray();
            
            var rating = GetRating(cardNames);
            if (mode == Mode.Joker && cardNames.Contains(Joker))
            {
                foreach (var card in cardValues.Keys.Where(c => c != Joker))
                {
                    var newRating = GetRating(cardNames.Replace(Joker, card));
                    if (newRating > rating)
                    {
                        rating = newRating;
                    }
                }
            }

            this.Rating = rating;
        }

        private static int GetRating(string cardNames)
        {
            var counts = new Dictionary<char, int>();
            foreach (var card in cardNames)
            {
                if (!counts.TryAdd(card, 1))
                {
                    counts[card]++;
                }
            }

            return counts.Count switch
            {
                1 => 6,
                2 when counts.Values.Any(c => c == 4) => 5, // four of a kind
                2 when counts.Values.Any(c => c == 3) => 4, // full house
                3 when counts.Values.Any(c => c == 3) => 3, // three of a kind
                3 => 2, // two pair
                4 => 1, // one pair
                _ => 0
            };
        }

        public int Rating { get; }
        
        public int CompareTo(Hand other)
        {
            if (this.Rating != other.Rating) return this.Rating.CompareTo(other.Rating);
            
            for (var i = 0; i < this.cards.Length; i++)
            {
                var result = this.cards[i].CompareTo(other.cards[i]);
                if (result != 0) return result;
            }

            return 0;
        }

        public override string ToString() => new(this.cards.Select(c => c.Name).ToArray());

        public bool Equals(Hand other)
        {
            if (other is null) return false;
            return ReferenceEquals(this, other) || this.cards.SequenceEqual(other.cards);
        }

        public override bool Equals(object obj) => obj is Hand h && this.Equals(h);

        public override int GetHashCode() => this.Rating;
    }
    
    [ExpectedResult(6440L, 247961593L)]
    public override object SolvePartOne() => this.GetTotalWinnings(Mode.Classic);

    [ExpectedResult(5905L, 248750699L)]
    public override object SolvePartTwo() => this.GetTotalWinnings(Mode.Joker);

    private long GetTotalWinnings(Mode mode)
    {
        var cardValues = GetCardValues(mode);
        
        var lines = this.GetInputLines();
        var handToBid = new Dictionary<Hand, int>();
        foreach (var line in lines)
        {
            var parts = line.Split(' ');
            var hand = new Hand(parts[0], cardValues, mode);
            handToBid.Add(hand, int.Parse(parts[1]));
        }
        
        var winnings = 0L;
        var rank = 1;
        var ranked = handToBid
            .OrderBy(h => h.Key.Rating)
            .ThenBy(h => h.Key);
        
        foreach (var pair in ranked)
        {
            winnings += pair.Value * rank;
            rank++;
        }
        
        return winnings;
    }

    private static Dictionary<char, Card> GetCardValues(Mode mode)
    {
        return new Dictionary<char, Card>
        {
            { '2', new Card('2', 2) },
            { '3', new Card('3', 3) },
            { '4', new Card('4', 4) },
            { '5', new Card('5', 5) },
            { '6', new Card('6', 6) },
            { '7', new Card('7', 7) },
            { '8', new Card('8', 8) },
            { '9', new Card('9', 9) },
            { 'T', new Card('T', 10) },
            { Joker, new Card(Joker, mode == Mode.Classic ? 11 : 1) },
            { 'Q', new Card('Q', 12) },
            { 'K', new Card('K', 13) },
            { 'A', new Card('A', 14) },
        };
    }

    protected override string GetTestInput(int? part = null)
    {
        return """
               32T3K 765
               T55J5 684
               KK677 28
               KTJJT 220
               QQQJA 483
               """;
    }
}