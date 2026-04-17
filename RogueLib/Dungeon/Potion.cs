using RogueLib.Utilities;
using System;

namespace RogueLib.Dungeon
{
    public class Potion : Item
    {
        public string PotionName { get; set; }

        public Potion(Vector2 pos, string name, string glyph, ConsoleColor color)
            : base(pos, glyph, color)
        {
            PotionName = name;
        }

        public override string Name => PotionName;
        public override string Description => "Restores health.";

        public override void Use(Player player)
        {
            int healAmount = 10;
            player.Heal(healAmount);

            Console.SetCursorPosition(0, 23);
            Console.Write($"You used a {Name} and healed {healAmount} HP!");
        }

        public override ItemDTO ToDTO() => new ItemDTO
        {
            Type = "Potion",
            Amount = 0
        };
    }
}