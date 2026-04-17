using System;
using System.Collections.Generic;
using System.Text;
using RogueLib.Interfaces;
using RogueLib.Utilities;

namespace RogueLib.Dungeon;

public abstract class Enemy : IDrawable, IDamageable
{
   public Vector2 Pos { get; set; }
   public char Glyph { get; init; }

   public int Health 
   { 
      get => field;
      set
      {
         if(value < 0)
         {
            field = 0;
         }
         else
         {
            field = value;
         }
      }
   }

   public string Name { get; init; }

   protected bool isAlive = true;
   public bool IsAlive
   {
      get => isAlive;
      set
      {
         if(Health == 0)
         {
            isAlive = false;
         }
      }
   }

   public Enemy(char c, Vector2 pos, string name)
   {
      Glyph = c;
      Pos = pos;
      Name = name;
   }
   public abstract void Draw(IRenderWindow disp);

   public abstract int Attack();

   public abstract void TakeDamage(int damage);

   
}
