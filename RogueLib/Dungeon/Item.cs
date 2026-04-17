using RogueLib.Utilities;
using System;

namespace RogueLib.Dungeon;

public abstract class Item : IDrawable
{
    public Vector2 Pos { get; set; }

    public string Glyph { get; }

    protected char _glyph;
    protected ConsoleColor _color;

    public ConsoleColor Color => _color;

    protected Item(Vector2 pos, char glyph, ConsoleColor color)
    {
        Pos = pos;
        _glyph = glyph;
        _color = color;
        Glyph = glyph.ToString();
    }

    protected Item(Vector2 pos, string glyph, ConsoleColor color)
    {
        Pos = pos;
        Glyph = glyph;
        _color = color;
    }

    public virtual void Use(Player player)
    {
    }

    public virtual string Name => GetType().Name;
    public virtual string Description => string.Empty;

    public class ItemDTO
    {
        public string Type { get; set; } = "";
        public int Amount { get; set; }
    }

    public virtual ItemDTO ToDTO() =>
        new ItemDTO { Type = GetType().Name };

    public static Item? FromDTO(ItemDTO dto)
    {
        return dto.Type switch
        {
            "XP" => new XP(Vector2.Zero, dto.Amount),
            "ManaPotion" => new ManaPotion(Vector2.Zero, dto.Amount <= 0 ? 10 : dto.Amount),
            "StrengthPotion" => new StrengthPotion(Vector2.Zero),
            _ => null
        };
    }

    public virtual void Draw(IRenderWindow disp)
    {
        disp.Draw(Glyph, Pos, Color);
    }
}