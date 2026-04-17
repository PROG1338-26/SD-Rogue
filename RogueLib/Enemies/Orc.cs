using RogueLib.Dungeon;
using System;
using System.Collections.Generic;
using System.Text;
using RogueLib.Utilities;

namespace RogueLib.Enemies;

public class Orc : Enemy
{
   public Orc(Vector2 pos) : base('O', pos, "Orc")
   {
      Health = 40;
   }

   public override void Draw(IRenderWindow disp)
   {
      disp.Draw(Glyph, Pos, ConsoleColor.DarkRed);
   }

   public override int Attack()
   {
      return 15;
   }

   public override void TakeDamage(int damage)
   {
      Health -= damage;
   }
}
