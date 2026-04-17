using RogueLib.Utilities;
using System;

namespace RogueLib.Dungeon
{
    public class XP : Item
    {
        private readonly int _amount;

        public XP(Vector2 pos, int amount, string glyph = "✨")
            : base(pos, glyph, ConsoleColor.Cyan)
        {
            _amount = amount;
        }

        public int Amount => _amount;
        public override string Name => "XP";
        public override string Description => $"Gives {Amount} XP.";

        public override void Use(Player player)
        {
            player.AddExp(_amount);
        }

        public override ItemDTO ToDTO() => new ItemDTO
        {
            Type = "XP",
            Amount = _amount
        };
    }
}