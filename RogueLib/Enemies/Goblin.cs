using System;
using System.Collections.Generic;
using System.Text;
using RogueLib.Dungeon;
using RogueLib.Utilities;

namespace RogueLib.Enemies;

public class Goblin : Enemy
{
   public Goblin(Vector2 pos) : base('G', pos, "Goblin")
   {
      Health = 12;
   }

   public override void Draw(IRenderWindow disp)
   {
      disp.Draw(Glyph, Pos, ConsoleColor.DarkRed);
   }

   public override int Attack()
   {
      return 2;
   }

   public override void TakeDamage(int damage)
   {
      Health -= damage;
   }
}
