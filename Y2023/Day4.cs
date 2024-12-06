using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Y2023;

[Year(2023)]
public class Day4 : Day<long>
{
    private sealed class Card
    {
        private readonly Lazy<int> count;
        
        public Card(int id, string[] winning, string[] picked)
        {
            this.Id = id;
            this.count = new(() => picked.Intersect(winning).Count());
        }
        
        public int Id { get; }

        public int Count => this.count.Value;

        public int Points => (int)Math.Pow(2, this.Count - 1);
    }

    private readonly Dictionary<int, Card> cards = new();
    
    public Day4()
    {
        var options = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
        foreach (var line in this.GetInputLines())
        {
            var cardParts = line.Split(':', options);
            var id = int.Parse(cardParts[0]["Card".Length..].TrimStart());
            var onlyNumbers = cardParts[1];
            
            var parts = onlyNumbers.Split('|', options);
            var winning = parts[0].Split(' ', options);
            var picked = parts[1].Split(' ', options);
            this.cards.Add(id, new Card(id, winning, picked));
        }
    }
    
    public override long SolvePartOne()
    {
        var total = 0;
        foreach (var card in this.cards.Values)
        {
            total += card.Points;
        }
        
        return total;
    }

    public override long SolvePartTwo() => this.cards.Count + this.GetCopyCount(this.cards.Values);

    private long GetCopyCount(ICollection<Card> list)
    {
        var result = 0L;
        foreach (var card in list)
        {
            result += this.GetCopyCount(card);
        }

        return result;
    }
    
    private long GetCopyCount(Card card)
    {
        var count = card.Count;
        
        long result = count;
        var copies = Enumerable.Range(card.Id + 1, count).Select(id => this.cards[id]);
        foreach (var copy in copies)
        {
            result += this.GetCopyCount(copy);
        }

        return result;
    }

    protected override string GetTestInput(int? part = null)
    {
        return """
               Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53
               Card 2: 13 32 20 16 61 | 61 30 68 82 17 32 24 19
               Card 3:  1 21 53 59 44 | 69 82 63 72 16 21 14  1
               Card 4: 41 92 73 84 69 | 59 84 76 51 58  5 54 83
               Card 5: 87 83 26 28 32 | 88 30 70 12 93 22 82 36
               Card 6: 31 18 13 56 72 | 74 77 10 23 35 67 36 11
               """;
    }
}