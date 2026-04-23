using System.Text;
using System.Collections.Generic;

namespace RogueLib.Dungeon;

public class Inventory
{
    private int _weaponBonus = 0;
    private int _healingPotions = 0;
    private List<string> _weapons = new List<string>();

    public void AddWeapon(Weapon weapon)
    {
        _weaponBonus += weapon.BonusStrength;
        _weapons.Add($"{weapon.Name} (+{weapon.BonusStrength})");
    }

    public void AddHealingPotion()
    {
        _healingPotions++;
    }

    public bool UseHealingPotion()
    {
        if (_healingPotions > 0)
        {
            _healingPotions--;
            return true;
        }
        return false;
    }

    public int GetWeaponBonus()
    {
        return _weaponBonus;
    }

    public int GetHealingPotions()
    {
        return _healingPotions;
    }

    public string GetDisplayText(Player player)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("Inventory");
        sb.AppendLine("---------");
        sb.AppendLine($"Gold: {player._gold}");
        sb.AppendLine();

        sb.AppendLine("Weapons:");
        if (_weapons.Count == 0)
        {
            sb.AppendLine("- None");
        }
        else
        {
            foreach (string weapon in _weapons)
            {
                sb.AppendLine($"- {weapon}");
            }
        }

        sb.AppendLine();
        sb.AppendLine($"Healing Potions: {_healingPotions}");
        sb.AppendLine();
        sb.AppendLine($"Weapon Strength Bonus: +{_weaponBonus}");

        return sb.ToString();
    }
}