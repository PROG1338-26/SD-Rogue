using System;
using System.Collections.Generic;
using System.Text;
using RogueLib;
using RogueLib.Engine;
using SandBox01;

namespace SandBox01.Levels
{
    public class Rules : Game
    {
        List<string> rules = new List<string>()
        {
            "rule one: the player can move up, down, left, right with the arrow keys.",
            "\nrule two: to pass the level you must defeat the boss",
            "\nrule three: you can pick up items to help you defeat the boss",
            "\nrule four: you can use items to heal yourself or damage the boss",
            "\nrule five: you can enter inventory with 'i' and navigate it with up/down arrows",
            "\nrule six: you can use items in inventory with 'enter' key",
            "\nrule seven: you can exit inventory with 'esc' key",
            "\nrule eight: you can leave and save with 'q'"

        };
        public string GetRules()
        {
            return string.Join("\n", rules);
        }
    }
}
