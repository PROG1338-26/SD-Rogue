using RogueLib.Utilities;
using FilterSet = System.Collections.Generic.HashSet<RogueLib.Utilities.Vector2>;

namespace RogueLib.Dungeon;

public interface IRenderWindow {
    int Height { get; }
    int Width { get; }

    void Draw(string s,     ConsoleColor color);
   void Draw(string s,     Vector2      offset, ConsoleColor color);
   void Draw(char   glyph, Vector2      pos,    ConsoleColor color);

   void fDraw(FilterSet fs, string s, ConsoleColor color);
   void Display();
}
