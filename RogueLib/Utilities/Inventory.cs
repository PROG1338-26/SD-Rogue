using System.Text;

namespace RogueLib.Dungeon;

public class Inventory
{
    private int _weaponBonus = 0;

    public void AddWeapon(Weapon weapon)
    {
        _weaponBonus += weapon.BonusStrength;
    }

    public int GetWeaponBonus()
    {
        return _weaponBonus;
    }

    public string GetDisplayText(Player player)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("Inventory");
        sb.AppendLine("---------");
        sb.AppendLine($"Gold: {player._gold}");
        sb.AppendLine();

        sb.AppendLine($"Weapon Strength Bonus: +{_weaponBonus}");

        return sb.ToString();
    }
}