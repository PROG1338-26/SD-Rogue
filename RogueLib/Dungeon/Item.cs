
using RogueLib.Utilities;
namespace RogueLib.Dungeon;
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
