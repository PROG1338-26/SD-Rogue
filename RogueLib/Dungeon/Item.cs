using RogueLib.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace RogueLib.Dungeon;

public abstract class Item : IDrawable {

    public Vector2 Pos { get; set; }
    public char Glyph => _glyph;
    protected char _glyph;
    protected ConsoleColor _color;

    public Item(char gly, Vector2 pos, ConsoleColor color = ConsoleColor.Yellow) {
    
        _color = color;
        _glyph = gly;
        Pos = pos;
    }
    public void Draw(IRenderWindow disp) {
        disp.Draw(_glyph, Pos, _color);
    }
}
