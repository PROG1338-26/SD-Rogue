using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace RogueLib.Dungeon
{
    public abstract class Item : IDrawable
    {
        public Vector2 Pos { get; set; }
        public char Glyph { get; init; }

        public Item(char c, Vector2 pos)
        {
            Glyph = c;
            Pos = pos;
        }

        public abstract void Draw(IRenderWindow disp);

    }
}
