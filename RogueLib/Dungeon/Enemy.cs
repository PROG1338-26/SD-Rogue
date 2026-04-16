using System;
using System.Collections.Generic;
using System.Text;
using RogueLib.Utilities;

namespace RogueLib.Dungeon;

public abstract class Enemy : IDrawable
{
   public Vector2 Pos { get; set; }
   public char Glyph { get; init; }

   
   public int Health { get; set; } = 12;

   public string Name { get; init; }

   public Enemy(char c, Vector2 pos, string name)
   {
      Glyph = c;
      Pos = pos;
      Name = name;
   }
   public abstract void Draw(IRenderWindow disp);

   
}
