namespace RogueLib.Dungeon;

public interface IActor {
  void Update();
  char Glyph { get; }
}