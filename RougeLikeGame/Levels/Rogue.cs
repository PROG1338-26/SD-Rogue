
using RogueLib.Dungeon;

namespace RogueLib.Utilities;
public abstract class RogueClass : Player {
    public string ClassName { get; }
    public string Description { get; }
    protected int _selectedIndex;
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
    public void Add(Item item)
    {
        if (item == null) return;
        _inventory.Add(item);
    }

    protected readonly Inventory _inventory = new Inventory();
    public bool Remove(Item item) => _inventory.Remove(item);
    public Inventory Inventory => _inventory;
    public IReadOnlyList<Item> Items => _inventory.Items;
    protected void FadeOutInventory(int start)
    {
        for (int opacity = 0; opacity < 3; opacity++)
        {
            for (int row = start; row <= start + 14; row++)
            {
                Console.SetCursorPosition(0, row);
                Console.Write(new string(' ', Console.WindowWidth));
            }
            Thread.Sleep(30);
        }
    }
    public override void ShowInventory()
    {
        int start = 5;
        ConsoleKey key;

        do
        {
            DrawInventoryWindow(start);

            key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.UpArrow && _selectedIndex > 0)
                _selectedIndex--;
            if (key == ConsoleKey.DownArrow && _selectedIndex < Items.Count - 1)
                _selectedIndex++;

            if (key == ConsoleKey.Enter && Items.Count > 0)
            {
                // Example action — you can expand this later
                Console.SetCursorPosition(0, start + 16);
                Console.WriteLine($"You selected: {Items[_selectedIndex].Name}");
                Console.ReadKey(true);
            }

        } while (key != ConsoleKey.Escape);
        FadeOutInventory(start);
    }
    protected void DrawInventoryWindow(int start)
    {

        for (int row = start; row <= start + 14; row++)
        {
            Console.SetCursorPosition(0, row);
            Console.Write(new string(' ', Console.WindowWidth));
        }

        Console.SetCursorPosition(0, start);
        Console.WriteLine("┌──────────────────────────────────────────┐");
        Console.SetCursorPosition(0, start + 1);
        Console.WriteLine("│              === INVENTORY ===           │");
        Console.WriteLine("│                                          │");
        int line = start + 3;

        if (Items.Count == 0)
        {
            Console.SetCursorPosition(0, line);
            Console.WriteLine("│                 (empty)                  │");
            line++;
        }
        else
        {
            for (int i = 0; i < Items.Count; i++)
            {
                Console.SetCursorPosition(0, line);

                bool selected = (i == _selectedIndex);

                if (selected)
                    Console.ForegroundColor = ConsoleColor.Yellow;

                Console.WriteLine($"│ {(selected ? ">" : " ")} {Items[i].Name,-18} {Items[i].Description,-19} │");

                Console.ResetColor();
                line++;
            }
        }

        while (line < start + 14)
        {
            Console.SetCursorPosition(0, line);
            Console.WriteLine("│                                          │");
            line++;
        }

        Console.SetCursorPosition(0, start + 14);
        Console.WriteLine("└──────────────────────────────────────────┘");

        Console.SetCursorPosition(0, start + 16);
        Console.WriteLine("Use ↑ ↓ to navigate, Enter to select, Esc to exit.");

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
