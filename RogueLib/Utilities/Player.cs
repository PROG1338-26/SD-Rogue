using RogueLib.Dungeon;

namespace RogueLib.Utilities;

public abstract class Player : IActor, IDrawable
{


    public string Name { get; set; }
    public Vector2 Pos;
    public string Glyph => "☺";
    public ConsoleColor _color = ConsoleColor.White;

    protected int _level = 0;
    protected int _hp = 12;
    protected int _str = 16;
    protected int _arm = 4;
    protected int _exp = 0;
    protected int _gold = 0;
    protected int _maxHp = 12;
    protected int _maxStr = 16;
    protected int _turn = 0;
 

    public int Turn => _turn;
    public Player(string name, string className)
    {
        Name = name;
        RogueClass = className;
        Pos = Vector2.Zero;
    }
    public string HUD =>
       $" Class: {RogueClass}  Level:{_level}  Gold: {_gold}    Hp: {_hp}({_maxHp})" +
       $"  Str: {_str}({_maxStr})" +
       $"  Arm: {_arm}   Exp: {_exp}/{10} Turn: {_turn}";

    // expose gold for saving/loading
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
    public virtual void Update()
    {
        _turn++;
    }
    public virtual void Draw(IRenderWindow disp)
    {
        disp.Draw(Glyph, Pos, _color);
    }
    public virtual void TakeDamage(int damage)
    {
        int effectiveDamage = Math.Max(0, damage - _arm);
        _hp -= effectiveDamage;
        if (_hp <= 0)
        {
            _hp = 0;
            // Handle player death (e.g., trigger game over)
        }
    }

    public abstract void ShowInventory();

    public string? RogueClass { get; set; }
    public int Strength => _str;

   

 
}