using RogueLib.Utilities;
using System;

namespace RogueLib.Dungeon
{
    public class StrengthPotion : Item
    {
        public StrengthPotion(Vector2 pos)
            : base(pos, "💪", ConsoleColor.Magenta)
        {
        }

        public override string Name => "Strength Potion";
        public override string Description => "Temporarily boosts strength.";

        public override void Use(Player player)
        {
            player.ApplyStrengthBuff(2, 5);
            Console.SetCursorPosition(0, 23);
            Console.Write($"You used a {Name} and gained strength!");
        }

        public override ItemDTO ToDTO() => new ItemDTO
        {
            Type = "StrengthPotion",
            Amount = 0
        };
    }
}