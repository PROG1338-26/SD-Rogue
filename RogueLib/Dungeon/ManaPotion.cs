using RogueLib.Utilities;
using System;

namespace RogueLib.Dungeon
{
    public class ManaPotion : Item
    {
        private readonly int _amount;

        public ManaPotion(Vector2 pos, int amount)
            : base(pos, "🔵", ConsoleColor.Blue)
        {
            _amount = amount;
        }

        public int Amount => _amount;
        public override string Name => "Mana Potion";
        public override string Description => $"Restores {_amount} mana.";

        public override void Use(Player player)
        {
            player.RestoreMana(_amount);
            Console.SetCursorPosition(0, 23);
            Console.Write($"You used a {Name} and restored {_amount} mana!");
        }

        public override ItemDTO ToDTO() => new ItemDTO
        {
            Type = "ManaPotion",
            Amount = _amount
        };
    }
}