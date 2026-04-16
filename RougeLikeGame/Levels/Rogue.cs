
using RogueLib.Dungeon;
using RogueLib.Engine;
using System.Text.RegularExpressions;

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
    private List<(string Name, int Count, string Description, string Glyph, ConsoleColor Color)> GetGroupedItems()
    {
        return Items
            .GroupBy(i => i.Name)
            .Select(g => (
                Name: g.Key,
                Count: g.Count(),
                Description: g.First().Description,
                Glyph: g.First().Glyph,   // now string
                Color: g.First().Color
            ))
            .ToList();
    }
    public override void Add(Item item)
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
            var grouped = GetGroupedItems();

            if (grouped.Count == 0)
            {
                DrawInventoryWindow(start);
                Console.SetCursorPosition(0, start + 16);
                Console.WriteLine("Inventory empty.");
                Console.ReadKey(true);
                FadeOutInventory(start);
                return;
            }

            // Clamp selection
            if (_selectedIndex >= grouped.Count)
                _selectedIndex = grouped.Count - 1;

            DrawInventoryWindow(start);

            key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.UpArrow && _selectedIndex > 0)
                _selectedIndex--;

            if (key == ConsoleKey.DownArrow && _selectedIndex < grouped.Count - 1)
                _selectedIndex++;

            if (key == ConsoleKey.Enter)
            {
                var selected = grouped[_selectedIndex];

                // Find one matching item
                var item = Items.FirstOrDefault(i => i.Name == selected.Name);

                if (item != null)
                {
                    Console.SetCursorPosition(0, start + 16);
                    Console.WriteLine($"You used: {item.Name}");
                    item.Use(this);
                    Remove(item);
                }

                Console.ReadKey(true);
            }

        } while (key != ConsoleKey.Escape);

        FadeOutInventory(start);
    }


    protected void DrawInventoryWindow(int start)
    {
        // Clear the whole panel area
        for (int row = start; row <= start + 14; row++)
        {
            Console.SetCursorPosition(0, row);
            Console.Write(new string(' ', Console.WindowWidth));
        }

        // Top border
        Console.SetCursorPosition(0, start);
        Console.WriteLine("┌──────────────────────────────────────────┐");

        // Title
        Console.SetCursorPosition(0, start + 1);
        Console.WriteLine("│              === INVENTORY ===           │");

        // Spacer
        Console.SetCursorPosition(0, start + 2);
        Console.WriteLine("│                                          │");

        int line = start + 3;

        var grouped = GetGroupedItems();

        for (int i = 0; i < grouped.Count; i++)
        {
            var item = grouped[i];
            bool selected = (i == _selectedIndex);

            Console.SetCursorPosition(0, line);

            // Arrow
            string arrow = selected ? ">" : " ";

            // Icon (always white so emojis show)
            string icon = item.Glyph;

            // Name + count
            string namePart = $"{item.Name} x{item.Count}";

            // Description
            string descPart = item.Description;

            // Highlight color only for icon + name
            if (selected)
                Console.ForegroundColor = item.Color;

            Console.Write($"│ {arrow} ");

            // Icon
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(icon);
            Console.ResetColor();

            // Name (18 chars)
            Console.ForegroundColor = selected ? item.Color : ConsoleColor.Gray;
            Console.Write($" {namePart,-18}");
            Console.ResetColor();

            // Description (17 chars)
            Console.Write($" {descPart,-17} │");

            line++;
        }

        // Fill remaining rows
        while (line < start + 14)
        {
            Console.SetCursorPosition(0, line);
            Console.WriteLine("│                                          │");
            line++;
        }

        // Bottom border
        Console.SetCursorPosition(0, start + 14);
        Console.WriteLine("└──────────────────────────────────────────┘");

        // Instructions
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
