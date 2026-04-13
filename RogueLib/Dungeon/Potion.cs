using RogueLib.Dungeon;
using RogueLib.Engine;
using RogueLib.Utilities;
using System;

namespace RougeLikeGame.Levels
{
    public class Potion : Item
    {
        public string PotionName { get; set; }

        public Potion(Vector2 pos, string name, char glyph, ConsoleColor color)
            : base(glyph, pos, color)
        {
            PotionName = name;
        }

        public override string Name => PotionName;

        public override string Description => "healing.";
    }
}
