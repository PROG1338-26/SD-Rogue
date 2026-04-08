using System.Data;

namespace RogueLib.Dungeon;

public interface IActor {
  char Glyph { get; }

    void Update();
}