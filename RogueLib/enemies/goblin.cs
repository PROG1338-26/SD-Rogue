using RogueLib.Dungeon;
using RogueLib.Engine;
using RogueLib.Utilities;
using System;

namespace RogueLib.enemies
{
    public class Goblin : NPC
    {
        public int Health { get; set; }

        public Goblin(Vector2 pos, string name, int health)
            : base(pos, name, '☠', ConsoleColor.DarkGreen, health, 5)
        {
            Health = health;
        }

        public new void TakeDamage(int damage)
        {
            Health -= damage;
        }
        public override void Update()
        {
            if ((PlayerRef.Pos - Pos).RookLength == 1)
            {
                PlayerRef.TakeDamage(3);
                return;
            }
        }
    }
}