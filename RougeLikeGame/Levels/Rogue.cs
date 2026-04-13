

namespace RogueLib.Utilities;
public abstract class RogueClass : Player {
    public string ClassName { get; }
    public string Description { get; }

    protected RogueClass(string name, string description, int hp, int str, int arm)
      : base(name, name)
    {
        ClassName = name;
        Description = description;

        _maxHp = hp;
        _hp = hp;
        _maxStr = str;
        _str = str;
        _arm = arm;
        Name = name;
    }


    public override string ToString() => $"{ClassName}: {Description} (HP:{_hp}, Str:{_str}, Arm:{_arm})";
}

// Concrete archetypes

//character classes with different stats and playstyles
public class Paladin : RogueClass {
    public Paladin() : base("Paladin", "skilled melee fighter. High HP and armour.", hp: 20, str: 18, arm: 6) { }
}

public class Rogue : RogueClass {
    public Rogue() : base("Rogue", "Fast and stealthy. Medium HP and locate items easiliy.", hp: 14, str: 14, arm: 3) { }
}

public class Wizard : RogueClass {
    public Wizard() : base("Wizard", "Fragile spellcaster. High damage potential.", hp: 12, str: 20, arm: 2) { }
}
public class Barbarian : RogueClass
{
    public Barbarian() : base("Barbarian", "Brute force fighter. High HP and strength, but low armour.", hp: 22, str: 22, arm: 1) { }
}
public class Archer : RogueClass
{
    public Archer() : base("Archer", "Ranged attacker. Medium HP and strength, but can attack from a distance.", hp: 16, str: 16, arm: 2) { }
}
// Simple factory / helper to list and create classes
public static class RogueFactory {
    public static string[] GetOptions() => new[] { "Paladin", "Rogue", "Wizard", "Barbarian", "Archer" };
    public static RogueClass Create(string option) {
        return option switch {
            "Barbarian" => new Barbarian(),
            "Paladin" => new Paladin(),
            "Rogue" => new Rogue(),
            "Wizard" => new Wizard(),
            "Archer" => new Archer(),
            _ => new Rogue(),
        };
    }
}
