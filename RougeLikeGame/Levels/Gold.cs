using RogueLib.Dungeon;
using RogueLib.Utilities;
using System;

namespace RogueLib.Dungeon
{
    public class Gold : Item
    {
        private readonly int _amount;

        public Gold(Vector2 pos, int amount, string glyph = "💰")
            : base(pos, glyph, ConsoleColor.Yellow)
        {
            _amount = amount;
        }

        public int Amount => _amount;
        public override string Name => "Gold";
        public override string Description => $"A pile of {Amount} gold coins.";

        public override ItemDTO ToDTO() => new ItemDTO
        {
            Type = "Gold",
            Amount = _amount
        };
    }
}