using System;
using System.Collections.Generic;
using System.Text;
using RogueLib.Dungeon;
using RogueLib.Utilities;

namespace SandBox01.Levels;

public class Gold : Item {

    public int amount { get; init; }
    public Gold(Vector2 pos, int amt) : base ('*',pos, ConsoleColor.Yellow) {
        amount = amt;
    }

}
