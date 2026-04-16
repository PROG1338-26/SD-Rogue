using RogueLib.Engine;
using RogueLib.Utilities;

namespace RogueLib.Dungeon;

public class HealingPotion : Item
{
    public int HealAmount { get; set; }

    public HealingPotion(Vector2 pos, int healAmount = 5) : base('!', pos)
    {
        HealAmount = healAmount;
    }

    public override void Draw(IRenderWindow disp)
    {
        disp.Draw("!", Pos, ConsoleColor.Red);
    }
}