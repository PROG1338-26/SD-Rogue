using RogueLib.Dungeon;
using RogueLib.Utilities;

public abstract class Player : IActor, IDrawable
{
    public string Name { get; set; }
    public Vector2 Pos;

    public char Glyph => '@';
    public ConsoleColor _color = ConsoleColor.White;

    public Inventory Inventory { get; set; }

    protected int _level = 0;
    protected int _hp = 12;
    protected int _str = 16;
    protected int _arm = 4;
    protected int _exp = 0;
    public int _gold = 0;

    protected int _maxHp = 12;
    protected int _maxStr = 16;
    protected int _turn = 0;

    public int Turn => _turn;

    // Strength total agora inclui bonus acumulado das weapons
    public int TotalStrength => _str + Inventory.GetWeaponBonus();

    public Player()
    {
        Name = "Rogue";
        Pos = Vector2.Zero;
        Inventory = new Inventory();
    }

    public string HUD =>
        $"Level:{_level}  Gold: {_gold}" +
        $"    Hp: {_hp}({_maxHp})" +
        $"  Str: {TotalStrength}({_maxStr})" +
        $"  Arm: {_arm}" +
        $"   Exp: {_exp}/10" +
        $"   Turn: {_turn}";

    public virtual void Update()
    {
        _turn++;
    }

    public virtual void Draw(IRenderWindow disp)
    {
        disp.Draw(Glyph, Pos, _color);
    }
}