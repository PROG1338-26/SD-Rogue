using RogueLib.Dungeon;
using RogueLib.Engine;
using RogueLib.Utilities;
using System;

namespace RogueLib.enemies
{
    public class NPC : IActor, IDrawable
    {
        public Vector2 Pos { get; set; }
        public Player PlayerRef { get; set; }
        public string Name { get; set; }
        public char Glyph { get; set; }
        public ConsoleColor Color { get; set; }
        public int HP { get; set; }
        public int Damage { get; set; }

        public NPC(Vector2 pos, string name, char glyph, ConsoleColor color, int hp, int damage)
        {
            Pos = pos;
            Name = name;
            Glyph = glyph;
            Color = color;
            HP = hp;
            Damage = damage;
        }
    
        public virtual void Update()
        {
            var rng = new Random();
            if (rng.Next(4) != 0) return;

            Vector2[] dirs = { Vector2.N, Vector2.S, Vector2.E, Vector2.W };
            var dir = dirs[rng.Next(dirs.Length)];

            Pos += dir;

            if (PlayerRef != null && (Pos - PlayerRef.Pos).RookLength == 1)
            {
                PlayerRef.TakeDamage(Damage);
                Console.WriteLine($"{Name} hits you for {Damage} damage!");
            }
        }

        public void Draw(IRenderWindow disp)
        {
            disp.Draw(Glyph, Pos, Color);
        }

        public void TakeDamage(int dmg)
        {
            HP -= dmg;
        }
    }
}
