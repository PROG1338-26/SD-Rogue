using RogueLib.Dungeon;
using RogueLib.Utilities;
using RogueLib.Engine;
using System;

namespace RogueLib.Dungeon;

public class Weapon : Item
{
    public string Name { get; set; }
    public int BonusStrength { get; set; }

    public Weapon(string name, int bonusStrength, Vector2 pos) : base(')', pos)
    {
        Name = name;
        BonusStrength = bonusStrength;
    }

    public override void Draw(IRenderWindow disp)
    {
        disp.Draw(Glyph, Pos, ConsoleColor.Cyan);
    }
}