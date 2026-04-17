using RogueLib.Dungeon;

namespace RogueLib.Utilities;

public abstract class Player : IActor, IDrawable
{
    public string Name { get; set; }
    public Vector2 Pos;
    public string Glyph => "☺";
    public ConsoleColor _color = ConsoleColor.White;

    private int _buffTurns = 0;
    private int _buffStrength = 0;

    protected int _mana;
    protected int _maxMana = 20;
    protected int _level = 0;
    protected int _hp = 12;
    protected int _str = 16;
    protected int _arm = 4;
    protected int _exp = 0;
    protected int _gold = 0;
    protected int _maxHp = 12;
    protected int _maxStr = 16;
    protected int _turn = 0;

    public Player(string name, string className)
    {
        Name = name;
        RogueClass = className;
        Pos = Vector2.Zero;
        _mana = _maxMana;
    }

    public string HUD =>
       $"[{RogueClass}] Lv:{_level} G:{_gold} HP:{_hp}/{_maxHp} MP:{_mana}/{_maxMana} " +
       $"Str:{_str} Arm:{_arm} XP:{_exp}/10 T:{_turn}";

    public int Gold
    {
        get => _gold;
        set => _gold = value;
    }

    public int Exp
    {
        get => _exp;
        set => _exp = value;
    }

    public int HP
    {
        get => _hp;
        set => _hp = value;
    }

    public int MaxHP
    {
        get => _maxHp;
        set => _maxHp = value;
    }

    public int Mana
    {
        get => _mana;
        set => _mana = value;
    }

    public int MaxMana
    {
        get => _maxMana;
        set => _maxMana = value;
    }

    public int Armour
    {
        get => _arm;
        set => _arm = value;
    }

    public int BaseStrength
    {
        get => _str;
        set => _str = value;
    }

    public int LevelNumber
    {
        get => _level;
        set => _level = value;
    }

    public int Turn
    {
        get => _turn;
        set => _turn = value;
    }

    public void AddGold(int amount)
    {
        if (amount <= 0) return;
        _gold += amount;
    }

    public void AddExp(int amount)
    {
        if (amount <= 0) return;
        _exp += amount;

        while (_exp >= 10)
        {
            _exp -= 10;
            LevelUp();
        }

        if (_exp < 0) _exp = 0;
    }

    protected virtual void LevelUp()
    {
        _level++;
        _maxHp += 2;
        _maxStr += 1;
        _hp = _maxHp;
        _str = _maxStr;
    }

    public void Heal(int amount)
    {
        _hp += amount;
        if (_hp > _maxHp)
            _hp = _maxHp;
    }

    public void RestoreMana(int amount)
    {
        _mana += amount;
        if (_mana > _maxMana)
            _mana = _maxMana;
    }

    public void ApplyStrengthBuff(int amount, int turns)
    {
        _buffStrength += amount;
        _buffTurns = turns;

        Console.SetCursorPosition(0, 23);
        Console.Write($"Strength increased by {amount} for {turns} turns!");
    }

    public virtual void Update()
    {
        _turn++;
        if (_buffTurns > 0)
        {
            _buffTurns--;
            if (_buffTurns == 0)
            {
                _buffStrength = 0;

                Console.SetCursorPosition(0, 23);
                Console.Write("Buff wore off.");
            }
        }
    }

    public virtual void Draw(IRenderWindow disp)
    {
        disp.Draw(Glyph, Pos, _color);
    }

    public virtual void TakeDamage(int damage)
    {
        int effectiveDamage = Math.Max(0, damage - _arm);
        _hp -= effectiveDamage;
        Console.SetCursorPosition(0, 22);
        Console.Write($"You took {damage} damage!");

        if (_hp <= 0)
        {
            Console.SetCursorPosition(0, 23);
            Console.Write("You died!");
        }
    }

    public abstract void ShowInventory();
    public abstract void Add(Item item);

    public string? RogueClass { get; set; }
    public int Strength => _str + _buffStrength;
}