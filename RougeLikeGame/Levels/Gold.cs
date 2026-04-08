using RogueLib.Dungeon;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;


namespace SandBox01.Levels
{
    internal class Gold : Item 
    {
        public int Amount { get; init; }

        public Gold(Vector2 pos, int amt) : base('*',pos)
        {
            Amount = amt;
        }

        public override void Draw(IRenderWindow disp)
        {
            disp.Draw(Glyph, Pos, ConsoleColor.Yellow);
        }

    }
}
